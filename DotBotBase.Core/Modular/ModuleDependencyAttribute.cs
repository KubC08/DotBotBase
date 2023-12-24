namespace DotBotBase.Core.Modular;

public class ModuleDependencyAttribute : Attribute
{
    public string GUID { get; set; }

    public ModuleDependencyAttribute(string guid)
    {
        GUID = guid;
    }
}