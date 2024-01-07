namespace DotBotBase.Core.Database;

/// <summary>
/// Used for defining a table column.
/// </summary>
public class DbColumnAttribute : Attribute
{
    /// <summary>
    /// The name or "key" of the column.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Is the column not nullable.
    /// </summary>
    public bool NotNullable { get; set; }
    /// <summary>
    /// Is the column a primary key.
    /// </summary>
    public bool PrimaryKey { get; set; }
    /// <summary>
    /// Are the values in the column unique.
    /// </summary>
    public bool IsUnique { get; set; }

    /// <summary>
    /// Define a table column for a database.
    /// </summary>
    /// <param name="name">The name or "key" of the column.</param>
    public DbColumnAttribute(string name)
    {
        Name = name;
        
        NotNullable = false;
        PrimaryKey = false;
        IsUnique = false;
    }
}