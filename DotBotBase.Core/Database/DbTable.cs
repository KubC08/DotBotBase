namespace DotBotBase.Core.Database;

public abstract class DbTable<T> : IDbTable
{
    public abstract string Name { get; }

    private DbTableProperties? _properties;
    public DbTableProperties Properties => _properties ??= DbTableProperties.Create(typeof(T));

    public async Task Create(T data) => await Create((object?)data);
    public async Task Update(Dictionary<string, object> filter, T newData) => await Update(filter, (object?)newData);
    public async Task UpdateByKey(string key, T newData) => await UpdateByKey(key, (object?)newData);
    public async Task<T?> GetGeneric(Dictionary<string, object> filter) => (T?)await Get(filter);
    public async Task<T?> GetGenericByKey(string key) => (T?)await GetByKey(key);

    public abstract Task Create(object? data);
    public abstract Task Update(Dictionary<string, object> filter, object? newData);
    public abstract Task UpdateByKey(string key, object? newData);
    public abstract Task Delete(string key);
    public abstract Task<object?> Get(Dictionary<string, object> filter);
    public abstract Task<object?> GetByKey(string key);
}