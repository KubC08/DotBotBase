using System.Reflection;
using DotBotBase.Core.Logging;

namespace DotBotBase.Core.Modular;

public static class ModuleService
{
    private static readonly Logger _log = new Logger("Module Service", DotBotInfo.Name);

    private static readonly Dictionary<Assembly, BotModule> _modules = new Dictionary<Assembly, BotModule>();
    public static BotModule[] Modules => _modules.Values.ToArray();

    public static void LoadLibrary(string assemblyPath)
    {
        if (!File.Exists(assemblyPath)) return;

        Assembly library = Assembly.LoadFile(assemblyPath);
        string assemblyName = library.GetName().ToString();
        
        _log.LogInfo($"Successfully loaded library {assemblyName}");
    }

    public static void LoadModule(string assemblyPath)
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
            
            _modules.Add(moduleAssembly, module);
            _log.LogInfo($"Successfully loaded {module.Name} v{module.Version} by {module.Author}");
        });
    }

    public static void StartModules()
    {
        _log.LogInfo("Starting all modules...");
        foreach (var entry in _modules)
        {
            if (entry.Value.IsRunning) continue;
            
            _log.SafeInvoke($"Failed to start {entry.Value.Name}", () =>
            {
                entry.Value.IsRunning = true;
                entry.Value.Start();
                _log.LogInfo($"Successfully start {entry.Value.Name}");
            });
        }
        _log.LogInfo("All modules started!");
    }

    public static void ShutdownModules()
    {
        _log.LogInfo("Shutting down all modules...");
        foreach (var entry in _modules)
        {
            if (!entry.Value.IsRunning) continue;
            
            _log.SafeInvoke($"Failed to shut down {entry.Value.Name}", () =>
            {
                entry.Value.IsRunning = false;
                entry.Value.Shutdown();
                _log.LogInfo($"Successfully shut down {entry.Value.Name}");
            });
        }
    }
}