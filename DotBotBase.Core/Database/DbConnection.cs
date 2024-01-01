namespace DotBotBase.Core.Database;

public abstract class DbConnection
{
    public abstract DbConnection Connect(string host, string database);
    public abstract void Disconnect();

    public virtual async Task<DbTable<T>?> GetOrCreateTable<T>(string name, T data) => await GetTable<T>(name) ?? await CreateTable(name, data);
    public abstract Task<DbTable<T>?> GetTable<T>(string name);
    
    public abstract Task<DbTable<T>?> CreateTable<T>(string name, T data);
    public abstract Task DeleteTable<T>(DbTable<T> table);
}