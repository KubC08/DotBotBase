namespace DotBotBase.Core.Database;

public interface IDbTable
{
    public string Name { get; }
    public DbTableProperties Properties { get; }
    
    public void Create(object? data);
    public void Update(Dictionary<string, object> filter, object? newData);
    public void UpdateByKey(string key, object? newData);
    public void Delete(string key);
    public object? Get(Dictionary<string, object> filter);
    public object? GetByKey(string key);
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