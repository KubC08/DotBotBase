using DotBotBase.Core.Logging;
using Newtonsoft.Json;

namespace DotBotBase.Core.Config;

/// <summary>
/// The service responsible for handling all the configurations of both the bot and modules.
/// </summary>
public static class ConfigService
{
    private static readonly Logger _log = new Logger("Config Service", DotBot.Name);

    /// <summary>
    /// If set to "true" the configuration service is setup and can be used. To setup use the "Setup" function.
    /// </summary>
    public static bool IsSetup { get; private set; } = false;
    /// <summary>
    /// The path to all the configuration files the service can access. To setup use the "Setup" function.
    /// </summary>
    public static string? ConfigLocation { get; private set; } = null;

    /// <summary>
    /// Sets up the configuration service so it can be used.
    /// </summary>
    /// <param name="configLocation">The location of all the configuration files.</param>
    public static void Setup(string configLocation)
    {
        if (IsSetup) return;

        ConfigLocation = configLocation;
        IsSetup = true;
    }
    
    /// <summary>
    /// Get a configuration of extended type "ISettings" and load the data from a given file.
    /// </summary>
    /// <param name="file">The target file containing the configuration data.</param>
    /// <typeparam name="T">The class that extends "ISettings" to deserialize the configuration into.</typeparam>
    /// <returns>The given configuration instance of extended type "ISettings"</returns>
    public static T? GetConfig<T>(string file) where T : ISettings
    {
        if (!file.EndsWith(".json")) file += ".json";
        if (!File.Exists(Path.Join(ConfigLocation, file)) || ConfigLocation == null) return default;
        
        _log.LogDebug($"Parsing file config {file}");
        string jsonData = File.ReadAllText(Path.Join(ConfigLocation, file));
        T? json = default;
        
        _log.SafeInvoke($"Failed to parse JSON for {file}", () =>
            json = JsonConvert.DeserializeObject<T>(jsonData));
        _log.LogDebug($"Parsed file config {file}");
        return json;
    }

    /// <summary>
    /// Sets or "generates" the configuration file of the specified object.
    /// </summary>
    /// <param name="obj">The object to serialize into the configuration file (has to extend "ISettings")</param>
    /// <param name="file">The file name to use for creating/setting the configuration file.</param>
    public static void SetConfig(object? obj, string file)
    {
        if (!file.EndsWith(".json")) file += ".json";
        if (obj == null || ConfigLocation == null) return;
        if (obj is not ISettings) return;

        _log.LogDebug($"Saving JSON config to {file}");
        _log.SafeInvoke($"Failed to serialize or save JSON for {file}", () =>
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(Path.Join(ConfigLocation, file), json);
        });
        _log.LogDebug($"Saved JSON config to {file}");
    }

    /// <summary>
    /// Uses the function "GetConfig" if the specified file exists, or creates a default version of the config file using "SetConfig".
    /// </summary>
    /// <param name="file">The target configuration file to load/set</param>
    /// <typeparam name="T">The class that extends "ISettings" to use as the configuration base.</typeparam>
    /// <returns>The configuration instance either filled with defaults, or with existing configurations.</returns>
    public static T? GetOrSetConfig<T>(string file) where T : ISettings, new()
    {
        if (!file.EndsWith(".json")) file += ".json";
        if (ConfigLocation == null) return default;
        
        T? config = GetConfig<T>(file);
        if (config == null)
        {
            T? defaultConfig = default;
            _log.SafeInvoke($"Failed to setup new config for {file}", () =>
            {
                defaultConfig = new T();
                defaultConfig.LoadDefaults();
            });
            
            SetConfig(defaultConfig, file);
            return defaultConfig;
        }
        return config;
    }
}