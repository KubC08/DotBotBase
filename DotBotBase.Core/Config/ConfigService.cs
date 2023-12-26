using DotBotBase.Core.Logging;
using Newtonsoft.Json;

namespace DotBotBase.Core.Config;

public static class ConfigService
{
    private static readonly Logger _log = new Logger("Config Service", DotBotInfo.Name);

    public static bool IsSetup { get; private set; } = false;
    public static string? ConfigLocation { get; private set; } = null;

    public static void Setup(string configLocation)
    {
        if (IsSetup) return;

        ConfigLocation = configLocation;
        IsSetup = true;
    }
    
    public static T? GetConfig<T>(string file) where T : ISettings
    {
        if (!file.EndsWith(".json")) file += ".json";
        if (!File.Exists(file) || ConfigLocation == null) return default;
        
        _log.LogDebug($"Parsing file config {file}");
        string jsonData = File.ReadAllText(Path.Join(ConfigLocation, file));
        T? json = default;
        
        _log.SafeInvoke($"Failed to parse JSON for {file}", () =>
            json = JsonConvert.DeserializeObject<T>(jsonData));
        _log.LogDebug($"Parsed file config {file}");
        return json;
    }

    public static void SetConfig(object? obj, string file)
    {
        if (!file.EndsWith(".json")) file += ".json";
        if (obj == null || ConfigLocation == null) return;
        if (obj is not ISettings) return;

        _log.LogDebug($"Saving JSON config to {file}");
        _log.SafeInvoke($"Failed to serialize or save JSON for {file}", () =>
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(Path.Join(file), json);
        });
        _log.LogDebug($"Saved JSON config to {file}");
    }

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