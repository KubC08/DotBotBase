namespace DotBotBase.Core.Database;

public abstract class DbTable<T> where T : new()
{
    public abstract string Name { get; }

    private DbTableProperties? _properties;
    public DbTableProperties Properties => _properties ??= DbTableProperties.Create(typeof(T));

    public async Task Create(T? data)
    {
        if (data == null) return;
        await Create((object)data);
    }
    public async Task Update(Dictionary<string, object> filter, T? newData)
    {
        if (newData == null) return;
        await Update(filter, (object)newData);
    }
    public async Task<T[]> GetGeneric(Dictionary<string, object> filter)
    {
        List<T> result = new List<T>();
        foreach (var entry in await Get(filter))
            result.Add((T)entry);
        return result.ToArray();
    }

    public abstract Task Create(object data);
    public abstract Task Update(Dictionary<string, object> filter, object newData);
    public abstract Task Delete(Dictionary<string, object> filter);
    public abstract Task<object[]> Get(Dictionary<string, object> filter);
}