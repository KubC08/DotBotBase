namespace DotBotBase.Core.Modular;

public class ModulePropertiesAttribute : Attribute
{
    public string GUID { get; set; }

    public ModulePropertiesAttribute(string guid)
    {
        GUID = guid;
    }
}