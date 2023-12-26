using Newtonsoft.Json;

namespace DotBotBase.Core.Config;

public interface ISettings
{
    [JsonIgnore]
    public string Version { get; }
    
    public void LoadDefaults();
}