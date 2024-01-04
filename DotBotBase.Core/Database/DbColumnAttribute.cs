namespace DotBotBase.Core.Database;

public class DbColumnAttribute : Attribute
{
    public string Name { get; }
    
    public bool NotNullable { get; set; }
    public bool PrimaryKey { get; set; }
    public bool IsUnique { get; set; }

    public DbColumnAttribute(string name)
    {
        Name = name;
        
        NotNullable = false;
        PrimaryKey = false;
        IsUnique = false;
    }
}