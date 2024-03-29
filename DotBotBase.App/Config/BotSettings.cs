using Newtonsoft.Json;

namespace DotBotBase.App.Config;

public class BotSettings : ISettings
{
    [JsonIgnore] public string Version { get; } = "1.0.0";
    
    public string? Token { get; set; }
    public bool AutoReconnect { get; set; }
    
    public void LoadDefaults()
    {
        Token = "";
        AutoReconnect = true;
    }
}