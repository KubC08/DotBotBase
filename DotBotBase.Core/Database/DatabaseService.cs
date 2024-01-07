using DotBotBase.Core.Logging;

namespace DotBotBase.Core.Database;

/// <summary>
/// The service that is used for handling all the database systems.
/// </summary>
public static class DatabaseService
{
    private static readonly Logger _log = new Logger("Database Service", DotBotInfo.Name);

    private static Type? _connectionHandler;
    private static string? _host;

    /// <summary>
    /// Is the connection handler setup, aka do you have a database extension.
    /// </summary>
    public static bool HasConnectionHandler => _connectionHandler != null;
    /// <summary>
    /// Is the host "url" or "path" setup.
    /// </summary>
    public static bool HasHost => _host != null;
    /// <summary>
    /// Are both connection handler and host properly setup so the database service can run.
    /// </summary>
    public static bool IsSetup => HasConnectionHandler && HasHost;

    /// <summary>
    /// Loads and sets up a connection handler aka database extension.
    /// This should only be called from the extension providing the connection handler.
    /// </summary>
    /// <param name="connectionHandler">The connection handler type passed from the extension, which must be an extension of DbConnection.</param>
    /// <exception cref="Exception">Returns exception if the connection handler does not extend DbConnection.</exception>
    public static void LoadConnectionHandler(Type connectionHandler)
    {
        if (_connectionHandler != null) return;
        if (!typeof(DbConnection).IsAssignableFrom(connectionHandler))
            throw new Exception("Invalid connection handler, must be DbConnection!");

        _connectionHandler = connectionHandler;
        _log.LogDebug($"Setup connection handler {connectionHandler.Name}");
    }
    
    /// <summary>
    /// Setup and load the host specified host.
    /// Note that this should only be called through the "App" portion of the bot.
    /// </summary>
    /// <param name="host">The host to target.</param>
    public static void LoadHost(string host)
    {
        if (_host != null) return;
        _host = host;
        _log.LogDebug($"Assigned host {host}");
    }

    /// <summary>
    /// Create a connection to a specified database.
    /// </summary>
    /// <param name="database">The database name.</param>
    /// <returns>The connection handler to interact with the database.</returns>
    /// <exception cref="Exception">Errors if the host or connection handler are not loaded, including if it cannot be created.</exception>
    public static DbConnection Connect(string database)
    {
        if (_connectionHandler == null)
            throw new Exception("No connection handler loaded!");
        if (_host == null)
            throw new Exception("Host is not initialized!");
        
        DbConnection? connection = (DbConnection?)Activator.CreateInstance(_connectionHandler);
        if (connection == null)
            throw new Exception("Could not create connection handler!");
        
        connection.Connect(_host, database);
        _log.LogInfo($"Established connection using {_connectionHandler.Name} to {_host}");
        return connection;
    }
}