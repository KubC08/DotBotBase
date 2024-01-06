using Newtonsoft.Json;

namespace DotBotBase.Core.Config;

/// <summary>
/// Use to specify a class that can be serialized and deserialized for configuration.
/// </summary>
public interface ISettings
{
    [JsonIgnore]
    public string Version { get; }
    
    /// <summary>
    /// Called when there is no existing configuration of the type and the defaults need to be loaded.
    /// </summary>
    public void LoadDefaults();
}