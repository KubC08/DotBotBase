namespace DotBotBase.Core.Database;

public abstract class DbTable<T> : IDbTable
{
    public abstract string Name { get; }

    private DbTableProperties? _properties;
    public DbTableProperties Properties => _properties ??= DbTableProperties.Create(typeof(T));

    public void Create(T data) => Create((object?)data);
    public void Update(Dictionary<string, object> filter, T newData) => Update(filter, (object?)newData);
    public void UpdateByKey(string key, T newData) => UpdateByKey(key, (object?)newData);
    public T? GetGeneric(Dictionary<string, object> filter) => (T?)Get(filter);
    public T? GetGenericByKey(string key) => (T?)GetByKey(key);

    public abstract void Create(object? data);
    public abstract void Update(Dictionary<string, object> filter, object? newData);
    public abstract void UpdateByKey(string key, object? newData);
    public abstract void Delete(string key);
    public abstract object? Get(Dictionary<string, object> filter);
    public abstract object? GetByKey(string key);
}