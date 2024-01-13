using System.Reflection;
using DotBotBase.Core.Logging;
using Mono.Cecil;

namespace DotBotBase.Core.Modular;

/// <summary>
/// The service that controls loading/unloading modules.
/// </summary>
public static class ModuleService
{
    private static readonly Logger _log = new Logger("Module Service", DotBot.Name);

    private static readonly Dictionary<string, string> _libraries = new Dictionary<string, string>();
    private static readonly List<BotModule> _modules = new List<BotModule>();
    /// <summary>
    /// All the loaded modules.
    /// </summary>
    public static BotModule[] Modules => _modules.ToArray();
    
    /// <summary>
    /// Setup and pre-load the libraries directory.
    /// </summary>
    /// <param name="targetPath">The path to target for pre-loading the libraries.</param>
    public static void SetupLibraries(string targetPath)
    {
        foreach (var libraryFile in Directory.GetFiles(targetPath, "*.dll", SearchOption.AllDirectories))
        {
            if (!BaseUtils.IsManagedAssembly(libraryFile))
            {
                _log.LogDebug($"The assembly {libraryFile} is not a managed assembly! Skipping...");
                continue;
            }

            _log.SafeInvoke($"Could not load {libraryFile}, attempting to load as native", () =>
            {
                AssemblyDefinition assemblyDef = AssemblyDefinition.ReadAssembly(libraryFile);
                _libraries.Add(assemblyDef.Name.FullName, libraryFile);
            });
        }
    }
    
    /// <summary>
    /// Event for resolving the custom libraries which were pre-loaded using the "SetupLibraries" function.
    /// </summary>
    public static Assembly? ResolveLibrary(object? sender, ResolveEventArgs args)
    {
        if (!_libraries.TryGetValue(args.Name, out var libraryPath)) return null;
        return Assembly.LoadFile(libraryPath);
    }

    /// <summary>
    /// Load all the modules in the specified directory.
    /// </summary>
    /// <param name="client">The DotBot client to attach the the selected modules to.</param>
    /// <param name="targetPath">The directory to load the modules from.</param>
    public static void LoadModules(DotBot client, string targetPath)
    {
        Dictionary<string, ModuleInfo> modules = new Dictionary<string, ModuleInfo>();
        Dictionary<string, ModuleInfo> extensions = new Dictionary<string, ModuleInfo>();
        foreach (var moduleFile in Directory.GetFiles(targetPath, "*.dll", SearchOption.AllDirectories))
        {
            ModuleDefinition moduleDef = ModuleDefinition.ReadModule(moduleFile);
            foreach (var typeDef in moduleDef.Types)
            {
                if (!typeDef.IsClass || typeDef.IsAbstract) continue;
                if (!typeDef.TryGetAttribute(typeof(ModulePropertiesAttribute), out var moduleProperties) || moduleProperties == null) continue;
                if (!moduleProperties.HasConstructorArguments || moduleProperties.ConstructorArguments.Count != 1) continue;

                string guid = (string)moduleProperties.ConstructorArguments[0].Value;
                bool isExtension = false;
                foreach (var property in moduleProperties.Properties)
                    if (property.Name == "IsExtension") isExtension = (bool)property.Argument.Value;

                List<string> dependencies = new List<string>();
                CustomAttribute[] dependencyAttributes = typeDef.GetCustomAttributes(typeof(ModuleDependencyAttribute));
                foreach (var dependencyAttribute in dependencyAttributes)
                    if (dependencyAttribute.HasConstructorArguments && dependencyAttribute.ConstructorArguments.Count == 1)
                        dependencies.Add((string)dependencyAttribute.ConstructorArguments[0].Value);
                foreach (var assemblyDependency in moduleDef.AssemblyReferences)
                    if (dependencies.Contains(assemblyDependency.FullName))
                        dependencies.Add(assemblyDependency.FullName);

                ModuleInfo info = new ModuleInfo()
                {
                    GUID = guid,
                    Dependencies = dependencies.ToArray(),
                    AssemblyPath = moduleFile
                };
                if (isExtension) extensions.Add(guid, info);
                modules.Add(guid, info);
                
                break; // Do not continue, we only support a single module class
            }
        }

        SortedDictionary<string, ModuleInfo> sortedModules = new SortedDictionary<string, ModuleInfo>();
        foreach (var entry in extensions)
            SortModules(sortedModules, modules, entry.Value);
        foreach (var entry in modules)
            SortModules(sortedModules, modules, entry.Value);

        foreach (var entry in sortedModules)
            LoadModule(client, entry.Value.AssemblyPath);
    }

    private static void SortModules(SortedDictionary<string, ModuleInfo> sortedModules, Dictionary<string, ModuleInfo> modules, ModuleInfo moduleInfo)
    {
        if (sortedModules.ContainsKey(moduleInfo.GUID)) return;
        foreach (var dependency in moduleInfo.Dependencies)
        {
            if (!modules.TryGetValue(dependency, out var dependencyModule))
                throw new MissingDependencyException(dependency);
            SortModules(sortedModules, modules, dependencyModule);
        }
        sortedModules.Add(moduleInfo.GUID, moduleInfo);
    }

    private static void LoadModule(DotBot client, string assemblyPath)
    {
        if (!File.Exists(assemblyPath)) return;

        Assembly moduleAssembly = Assembly.LoadFile(assemblyPath);
        Type? moduleType = moduleAssembly.GetTypes()
            .FirstOrDefault(mod => mod.IsClass && typeof(BotModule).IsAssignableFrom(mod));
        if (moduleType == null)
        {
            _log.LogWarning($"Could not find module type for {moduleAssembly.FullName}");
            return;
        }

        _log.SafeInvoke($"Failed to load module {moduleAssembly.FullName}", () =>
        {
            BotModule? module = (BotModule?)Activator.CreateInstance(moduleType);
            if (module == null) return;

            module.Log = new Logger(null, module.Name);
            module.Client = client;
            
            _modules.Add(module);
            _log.LogInfo($"Successfully loaded {module.Name} v{module.Version} by {module.Author}");
        });
    }

    /// <summary>
    /// Starts all the modules that were loaded using the "LoadModules" function.
    /// </summary>
    public static void StartModules()
    {
        _log.LogInfo("Starting all modules...");
        foreach (var entry in _modules)
        {
            if (entry.IsRunning) continue;
            
            _log.SafeInvoke($"Failed to start {entry.Name}", () =>
            {
                entry.IsRunning = true;
                entry.Start();
                Task task = entry.StartAsync();
                if (entry.ShouldWait) task.Wait();
                _log.LogInfo($"Successfully start {entry.Name}");
            });
        }
        _log.LogInfo("All modules started!");
    }

    /// <summary>
    /// Shuts down all currently loaded and running modules.
    /// </summary>
    public static void ShutdownModules()
    {
        _log.LogInfo("Shutting down all modules...");
        foreach (var entry in _modules)
        {
            if (!entry.IsRunning) continue;
            
            _log.SafeInvoke($"Failed to shut down {entry.Name}", () =>
            {
                entry.IsRunning = false;
                entry.Shutdown();
                Task task = entry.ShutdownAsync();
                if (entry.ShouldWait) task.Wait();
                _log.LogInfo($"Successfully shut down {entry.Name}");
            });
        }
    }

    private struct ModuleInfo
    {
        public string GUID;
        public string[] Dependencies;
        public string AssemblyPath;
    }
}