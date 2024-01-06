namespace DotBotBase.Core.Modular;

/// <summary>
/// Properties that specify properties for the pre-loading stage.
/// </summary>
/// <param name="guid">The GUID of the module.</param>
public class ModulePropertiesAttribute(string guid) : Attribute
{
    /// <summary>
    /// The GUID of the module
    /// </summary>
    public string GUID = guid;
    
    /// <summary>
    /// Is the module an extension or normal module.
    /// </summary>
    public bool IsExtension { get; set; }
}