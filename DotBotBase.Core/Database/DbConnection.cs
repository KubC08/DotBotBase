namespace DotBotBase.Core.Database;

/// <summary>
/// The connection to the database system.
/// Extend this class when making a custom database extension.
/// </summary>
public abstract class DbConnection : IDisposable
{
    public virtual void Dispose() => Disconnect();
    
    /// <summary>
    /// Connect to a specific host. (This method is called automatically by the DatabaseService).
    /// </summary>
    /// <param name="host">The target host url/path to connect to.</param>
    /// <param name="database">The database name for the specific connection.</param>
    /// <returns>The same connection after connection.</returns>
    public abstract DbConnection Connect(string host, string database);
    /// <summary>
    /// Disconnect from the host and database (must be connected).
    /// </summary>
    public abstract void Disconnect();

    /// <summary>
    /// Attempts to get existing table if it does exist, otherwise it creates the table using the given data.
    /// </summary>
    /// <param name="name">The name of the table.</param>
    /// <typeparam name="T">The table entry type.</typeparam>
    /// <returns>The DbTable instance of either the default or existing table.</returns>
    public virtual async Task<DbTable<T>?> GetOrCreateTable<T>(string name) where T : new() => await GetTable<T>(name) ?? await CreateTable<T>(name);
    /// <summary>
    /// Gets an existing table from a specific name.
    /// If it does not exist it returns null.
    /// </summary>
    /// <param name="name">The name of the table.</param>
    /// <typeparam name="T">The table entry type.</typeparam>
    /// <returns>The DbTable instance of the requested table or null if it does not exist.</returns>
    public abstract Task<DbTable<T>?> GetTable<T>(string name) where T : new();
    
    /// <summary>
    /// Creates a table if it does not exist and returns the DbTable instance.
    /// In case of issues it returns null.
    /// </summary>
    /// <param name="name">The name of the table.</param>
    /// <typeparam name="T">The table entry type to use as a creation basis.</typeparam>
    /// <returns>The DbTable instance that was created or null if there was an issue.</returns>
    public abstract Task<DbTable<T>?> CreateTable<T>(string name) where T : new();
    /// <summary>
    /// Delete a table if it exists.
    /// </summary>
    /// <param name="table">The table to delete.</param>
    /// <typeparam name="T">The table entry type of the DbTable.</typeparam>
    /// <returns>The task to indicate when the table is deleted.</returns>
    public abstract Task DeleteTable<T>(DbTable<T> table) where T : new();
}