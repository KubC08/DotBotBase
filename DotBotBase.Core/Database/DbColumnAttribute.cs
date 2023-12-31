namespace DotBotBase.Core.Database;

public class DbColumnAttribute : Attribute
{
    public string Name { get; }
    
    public string? CustomType { get; set; }
    public bool NotNullable { get; set; }

    public DbColumnAttribute(string name)
    {
        Name = name;

        CustomType = null;
        NotNullable = false;
    }
}