using System.Data;
using DotBotBase.Core.Database;
using System.Data.SQLite;

namespace DotBotBase.SQLite.Database;

public class DBSQLiteConnection : DbConnection
{
    private SQLiteConnection? _connection;
    
    public override DbConnection Connect(string host, string database)
    {
        if (!Directory.Exists(host)) Directory.CreateDirectory(host);
        
        _connection = new SQLiteConnection($"Data Source={Path.Join(host, database + ".db")};Version=3;New=True;Compress=True;");
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

        await using SQLiteCommand cmd = _connection.CreateCommand();
        
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
        
        return new DBSQLiteTable<T>(name, _connection);
    }

    public override async Task<DbTable<T>?> CreateTable<T>(string name)
    {
        if (_connection?.State != ConnectionState.Open) return null;

        DbTable<T> table = new DBSQLiteTable<T>(name, _connection);
        await using SQLiteCommand cmd = _connection.CreateCommand();
        
        cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {name} ({SQLiteUtils.GetColumnList(table.Properties)})";
        await cmd.ExecuteNonQueryAsync();

        return table;
    }

    public override async Task DeleteTable<T>(DbTable<T> table)
    {
        if (_connection?.State != ConnectionState.Open) return;

        await using SQLiteCommand cmd = _connection.CreateCommand();
        cmd.CommandText = $"DROP TABLE IF EXISTS {table.Name}";
        await cmd.ExecuteNonQueryAsync();
    }
}