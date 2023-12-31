using System.Reflection;

namespace DotBotBase.Core.Database;

public class DbTableProperties
{
    private readonly Dictionary<string, DbColumnAttribute> _columns = new Dictionary<string, DbColumnAttribute>();
    private readonly Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>();

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

    public DbColumnAttribute? GetColumn(string name) => _columns.GetValueOrDefault(name);
    public FieldInfo? GetField(string name) => _fields.GetValueOrDefault(name);
    public FieldInfo? GetField(DbColumnAttribute column) => _fields.GetValueOrDefault(column.Name);
}