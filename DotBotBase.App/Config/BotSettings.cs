using DotBotBase.Core.Config;
using Newtonsoft.Json;

namespace DotBotBase.App.Config;

public class BotSettings : ISettings
{
    [JsonIgnore] public string Version { get; } = "1.0.0";
    
    public string? Token { get; set; }
    public bool AutoReconnect { get; set; }
    public bool IsSharded { get; set; }
    public bool ShowDebugLogs { get; set; }
    
    public string? DatabaseHost { get; set; }
    
    public Dictionary<string, bool> ActiveModules { get; set; }
    
    public void LoadDefaults()
    {
        Token = "";
        AutoReconnect = true;
        IsSharded = false;
        ShowDebugLogs = false;
        
        DatabaseHost = "";

        ActiveModules = new Dictionary<string, bool>();
    }
}