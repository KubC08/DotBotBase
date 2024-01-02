namespace DotBotBase.Core.Database;

public abstract class DbConnection : IDisposable
{
    public virtual void Dispose() => Disconnect();
    
    public abstract DbConnection Connect(string host, string database);
    public abstract void Disconnect();

    public virtual async Task<DbTable<T>?> GetOrCreateTable<T>(string name) where T : new() => await GetTable<T>(name) ?? await CreateTable<T>(name);
    public abstract Task<DbTable<T>?> GetTable<T>(string name) where T : new();
    
    public abstract Task<DbTable<T>?> CreateTable<T>(string name) where T : new();
    public abstract Task DeleteTable<T>(DbTable<T> table) where T : new();
}