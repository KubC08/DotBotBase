using Newtonsoft.Json;

namespace DotBotBase.App.Config;

public interface ISettings
{
    [JsonIgnore]
    public string Version { get; }
    
    public void LoadDefaults();
}