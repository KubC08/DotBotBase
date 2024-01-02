using DotBotBase.Core.Config;
using Newtonsoft.Json;

namespace DotBotBase.App.Config;

public class BotSettings : ISettings
{
    [JsonIgnore] public string Version { get; } = "1.0.0";
    
    public string? Token { get; private set; }
    public bool AutoReconnect { get; private set; }
    public string? DatabaseHost { get; private set; }
    
    public void LoadDefaults()
    {
        Token = "";
        AutoReconnect = true;
        DatabaseHost = "";
    }
}