namespace DotBotBase.Core.Modular;

public class ModulePropertiesAttribute(string guid) : Attribute
{
    public string GUID = guid;
    public bool IsExtension { get; set; }
}