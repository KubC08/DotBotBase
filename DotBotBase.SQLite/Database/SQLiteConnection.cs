using System.Data;
using System.Reflection;
using DotBotBase.Core.Database;
using Microsoft.Data.Sqlite;

namespace DotBotBase.SQLite.Database;

public class SQLiteConnection : DbConnection
{
    private SqliteConnection? _connection;
    
    public override DbConnection Connect(string host, string database)
    {
        _connection = new SqliteConnection($"Data Source={Path.Join(host, database)};Version=3;New=True;Compress=True;");
        _connection.Open();
        
        return this;
    }
    public override void Disconnect()
    {
        if (_connection?.State != ConnectionState.Open) return;
        _connection.Close();
    }

    private async Task<bool> CheckIfTableExists(string name)
    {
        if (_connection?.State != ConnectionState.Open) return false;

        await using SqliteCommand cmd = _connection.CreateCommand();
        
        cmd.CommandText = "SELECT count(*) FROM sqlite_master WHERE type = 'table' AND name = $name";
        cmd.Parameters.AddWithValue("$name", name);
        object? result = await cmd.ExecuteScalarAsync();

        if (result == null) return false;
        return Convert.ToInt32(result) > 0;
    }

    public override async Task<DbTable<T>?> GetTable<T>(string name)
    {
        if (_connection?.State != ConnectionState.Open) return null;
        if (!await CheckIfTableExists(name)) return null;
        
        return new SQLiteTable<T>(name, _connection);
    }

    public override async Task<DbTable<T>?> CreateTable<T>(string name, T data)
    {
        if (_connection?.State != ConnectionState.Open) return null;

        DbTable<T> table = new SQLiteTable<T>(name, _connection);
        await using SqliteCommand cmd = _connection.CreateCommand();
        
        cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {name} ({SQLiteUtils.GetColumnList(table.Properties)})";
        await cmd.ExecuteNonQueryAsync();

        return table;
    }

    public override async Task DeleteTable<T>(DbTable<T> table)
    {
        if (_connection?.State != ConnectionState.Open) return;

        await using SqliteCommand cmd = _connection.CreateCommand();
        cmd.CommandText = $"DROP TABLE IF EXISTS {table.Name}";
        await cmd.ExecuteNonQueryAsync();
    }
}