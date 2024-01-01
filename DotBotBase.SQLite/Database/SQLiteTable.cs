using DotBotBase.Core.Database;
using Microsoft.Data.Sqlite;

namespace DotBotBase.SQLite.Database;

public class SQLiteTable<T> : DbTable<T>
{
    private SqliteConnection _connection;
    
    public override string Name { get; }

    public SQLiteTable(string name, SqliteConnection connection)
    {
        Name = name;
        
        _connection = connection;
    }
}