using System.Reflection;

namespace DotBotBase.Core.Database;

/// <summary>
/// The properties of a given table that allows you to easily create queries.
/// </summary>
public class DbTableProperties
{
    private readonly SortedDictionary<string, DbColumnAttribute> _columns = new SortedDictionary<string, DbColumnAttribute>();
    private readonly Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>();

    /// <summary>
    /// Create the table properties from a given table type.
    /// </summary>
    /// <param name="type">The table type to extract the properties from.</param>
    /// <returns>The table properties extracted from the selected type.</returns>
    public static DbTableProperties Create(Type type)
    {
        DbTableProperties properties = new DbTableProperties();

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            DbColumnAttribute? column = (DbColumnAttribute?)field.GetCustomAttribute(typeof(DbColumnAttribute));
            if (column == null) continue;

            properties._columns.TryAdd(column.Name, column);
            properties._fields.TryAdd(column.Name, field);
        }

        return properties;
    }

    /// <summary>
    /// Get the database columns setup in the table type from the properties.
    /// </summary>
    /// <returns></returns>
    public DbColumnAttribute[] GetColumns() => _columns.Values.ToArray();

    /// <summary>
    /// Get the column data put in the table type from name.
    /// </summary>
    /// <param name="name">The name of the column to find.</param>
    /// <returns>The column data put in the table type.</returns>
    public DbColumnAttribute? GetColumn(string name) => _columns.GetValueOrDefault(name);
    /// <summary>
    /// Get a FieldInfo from a given column name.
    /// </summary>
    /// <param name="name">The name of the column to find the field of.</param>
    /// <returns>The FieldInfo of the column.</returns>
    public FieldInfo? GetField(string name) => _fields.GetValueOrDefault(name);
    /// <summary>
    /// Get a FieldInfo from a given column data.
    /// </summary>
    /// <param name="column">The column to get the FieldInfo form.</param>
    /// <returns>The FieldInfo of the column.</returns>
    public FieldInfo? GetField(DbColumnAttribute column) => _fields.GetValueOrDefault(column.Name);
}