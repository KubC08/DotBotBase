namespace DotBotBase.Core.Database;

public interface IDbTable
{
    public string Name { get; }
    public DbTableProperties Properties { get; }
    
    public Task Create(object? data);
    public Task Update(Dictionary<string, object> filter, object? newData);
    public Task UpdateByKey(string key, object? newData);
    public Task Delete(string key);
    public Task<object?> Get(Dictionary<string, object> filter);
    public Task<object?> GetByKey(string key);
}

/*public interface IDbTable<TKey, TValue>
{
    public string Name { get; }
    
    // Database methods
    public void BuildTable();

    // Data methods
    public void Create(TValue data);
    public void Update(Dictionary<object, object> filter, TValue newData);
    public void UpdateByKey(TKey key, TValue newData);
    public void Delete(TKey key);
    public TValue? Get(Dictionary<object, object> filter);
    public TValue? GetByKey(TKey key);
}*/