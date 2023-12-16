using DotBotBase.Core;
using DotBotBase.Core.Logging;
using Newtonsoft.Json;

namespace DotBotBase.App.Config;

public static class ConfigService
{
    private static readonly Logger _log = new Logger("Config Service", DotBot.Name);
    
    public static T? GetConfig<T>(string file) where T : ISettings
    {
        if (!File.Exists(file)) return default;
        
        _log.LogDebug($"Parsing file config {file}");
        string jsonData = File.ReadAllText(file);
        T? json = default;
        
        _log.SafeInvoke($"Failed to parse JSON for {file}", () =>
            json = JsonConvert.DeserializeObject<T>(jsonData));
        _log.LogDebug($"Parsed file config {file}");
        return json;
    }

    public static void SetConfig(object? obj, string file)
    {
        if (obj == null) return;
        if (obj is not ISettings) return;

        _log.LogDebug($"Saving JSON config to {file}");
        _log.SafeInvoke($"Failed to serialize or save JSON for {file}", () =>
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(file, json);
        });
        _log.LogDebug($"Saved JSON config to {file}");
    }

    public static T? GetOrSetConfig<T>(string file) where T : ISettings, new()
    {
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