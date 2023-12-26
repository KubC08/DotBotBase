using System.Text.Json.Serialization;
using DotBotBase.Core.Config;

namespace DotBotBase.SQLite.Config;

public class ModuleSettings : ISettings
{
    [JsonIgnore] public string Version => "1.0.0";
    
    public string? FilePath { get; private set; }

    public void LoadDefaults()
    {
        FilePath = "";
    }
}