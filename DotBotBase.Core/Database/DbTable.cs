namespace DotBotBase.Core.Database;

/// <summary>
/// The table instance that allows you to interact with the database table.
/// </summary>
/// <typeparam name="T">The table entry type.</typeparam>
public abstract class DbTable<T> where T : new()
{
    /// <summary>
    /// The name of the table in the database.
    /// </summary>
    public abstract string Name { get; }

    private DbTableProperties? _properties;
    /// <summary>
    /// Get the properties of the database table.
    /// </summary>
    public DbTableProperties Properties => _properties ??= DbTableProperties.Create(typeof(T));

    /// <summary>
    /// Create a database table entry.
    /// </summary>
    /// <param name="data">The data to be added to the table.</param>
    public async Task Create(T? data)
    {
        if (data == null) return;
        await Create((object)data);
    }
    /// <summary>
    /// Update a database table entry.
    /// </summary>
    /// <param name="filter">The filter to target a specific entry/entries in the database.</param>
    /// <param name="newData">The new data to write into the targeted entry/entries.</param>
    public async Task Update(Dictionary<string, object> filter, T? newData)
    {
        if (newData == null) return;
        await Update(filter, (object)newData);
    }
    /// <summary>
    /// Get the entries from the database table based on a filter.
    /// </summary>
    /// <param name="filter">The filter to target specific entry/entries in the database.</param>
    /// <returns>The entries that are present in the database table with specific filter.</returns>
    public async Task<T[]> GetGeneric(Dictionary<string, object> filter)
    {
        List<T> result = new List<T>();
        foreach (var entry in await Get(filter))
            result.Add((T)entry);
        return result.ToArray();
    }

    /// <summary>
    /// Create a database table entry.
    /// </summary>
    /// <param name="data">The data to be added to the table.</param>
    /// <returns>The asynchronous task to indicate when the entry is created.</returns>
    public abstract Task Create(object data);
    /// <summary>
    /// Update a database table entry.
    /// </summary>
    /// <param name="filter">The filter to target a specific entry/entries in the database.</param>
    /// <param name="newData">The new data to write into the targeted entry/entries.</param>
    /// <returns>The asynchronous task to indicate when the entry is updated.</returns>
    public abstract Task Update(Dictionary<string, object> filter, object newData);
    /// <summary>
    /// Delete the entry from the database table.
    /// </summary>
    /// <param name="filter">The filter to target a specific entry/entries in the database.</param>
    /// <returns>The asynchronous task to indicate when the entry is deleted.</returns>
    public abstract Task Delete(Dictionary<string, object> filter);
    /// <summary>
    /// Get the entries from the database table based on a filter.
    /// </summary>
    /// <param name="filter">The filter to target specific entry/entries in the database.</param>
    /// <returns>The entries that are present in the database table with specific filter.</returns>
    public abstract Task<object[]> Get(Dictionary<string, object> filter);
}