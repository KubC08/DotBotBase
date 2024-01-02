namespace DotBotBase.Core.Modular;

public class ModulePropertiesAttribute(string guid) : Attribute
{
    public string GUID { get; set; } = guid;
    public bool IsExtension { get; set; }
}