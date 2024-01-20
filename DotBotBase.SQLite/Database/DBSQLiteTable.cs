using System.Data;
using System.Data.SQLite;
using System.Reflection;
using DotBotBase.Core.Database;

namespace DotBotBase.SQLite.Database;

public class DBSQLiteTable<T> : DbTable<T> where T : new()
{
    private SQLiteConnection _connection;
    
    public override string Name { get; }
    
    public DBSQLiteTable(string name, SQLiteConnection connection)
    {
        Name = name;
        
        _connection = connection;
    }
    
    public override async Task Create(object data)
    {
        if (_connection?.State != ConnectionState.Open) return;
        
        await using SQLiteCommand cmd = _connection.CreateCommand();

        string valueIndexes = "";
        string columnList = "";
        foreach (var column in Properties.GetColumns())
        {
            FieldInfo? field = Properties.GetField(column.Name);
            if (field == null) continue;
            
            object? value = field.GetValue(data);
            if (value == null) continue;

            if (columnList.Length > 0) columnList += ", ";
            columnList += column.Name;
            
            if (valueIndexes.Length > 0) valueIndexes += ", ";
            valueIndexes += $"${column.Name}";
            cmd.Parameters.AddWithValue("$" + column.Name, value);
        }
        
        cmd.CommandText = $"INSERT INTO {Name} ({columnList}) VALUES ({valueIndexes})";
        await cmd.ExecuteNonQueryAsync();
    }

    public override async Task Update(Dictionary<string, object> filter, object newData)
    {
        if (_connection?.State != ConnectionState.Open) return;

        await using SQLiteCommand cmd = _connection.CreateCommand();
        
        string where = "";
        foreach (var filterEntry in filter)
        {
            if (where.Length > 0) where += " AND ";
            where += $"{filterEntry.Key} = ${filterEntry.Key}";
            cmd.Parameters.AddWithValue("$" + filterEntry.Key, filterEntry.Value);
        }

        string sets = "";
        foreach (var column in Properties.GetColumns())
        {
            FieldInfo? field = Properties.GetField(column.Name);
            if (field == null) continue;
            
            object? value = field.GetValue(newData);
            if (value == null) continue;

            if (sets.Length > 0) sets += ", ";
            sets += $"{column.Name} = ${column.Name}";
            cmd.Parameters.AddWithValue("$" + column.Name, value);
        }

        cmd.CommandText = $"UPDATE {Name} SET {sets} WHERE {where}";
        await cmd.ExecuteNonQueryAsync();
    }

    public override async Task Delete(Dictionary<string, object> filter)
    {
        if (_connection?.State != ConnectionState.Open) return;

        await using SQLiteCommand cmd = _connection.CreateCommand();
        
        string where = "";
        foreach (var filterEntry in filter)
        {
            if (where.Length > 0) where += " AND ";
            where += $"{filterEntry.Key} = ${filterEntry.Key}";
            cmd.Parameters.AddWithValue("$" + filterEntry.Key, filterEntry.Value);
        }

        cmd.CommandText = $"DELETE FROM {Name} WHERE {where}";
        await cmd.ExecuteNonQueryAsync();
    }

    public override async Task<object[]> Get(Dictionary<string, object> filter)
    {
        if (_connection?.State != ConnectionState.Open) return Array.Empty<object>();

        await using SQLiteCommand cmd = _connection.CreateCommand();

        string where = "";
        foreach (var filterEntry in filter)
        {
            if (where.Length > 0) where += " AND ";
            where += $"{filterEntry.Key} = ${filterEntry.Key}";
            cmd.Parameters.AddWithValue("$" + filterEntry.Key, filterEntry.Value);
        }

        string selector = "";
        foreach (var column in Properties.GetColumns())
        {
            if (selector.Length > 0) selector += ", ";
            selector += column.Name;
        }

        cmd.CommandText = $"SELECT {selector} FROM {Name} WHERE {where}";

        List<object> result = new List<object>();
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                T entry = new T();

                DbColumnAttribute[] columns = Properties.GetColumns();
                for (int i = 0; i < columns.Length; i++)
                {
                    DbColumnAttribute column = columns[i];
                    FieldInfo? field = Properties.GetField(column.Name);
                    if (field == null) continue;
                    
                    field.SetValue(entry, reader.GetValue(i));
                }
                result.Add(entry);
            }
        }

        return result.ToArray();
    }
}