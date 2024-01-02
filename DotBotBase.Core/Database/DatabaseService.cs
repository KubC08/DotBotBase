using DotBotBase.Core.Logging;

namespace DotBotBase.Core.Database;

public static class DatabaseService
{
    private static readonly Logger _log = new Logger("Database Service", DotBotInfo.Name);

    private static Type? _connectionHandler;
    private static string? _host;

    public static bool HasConnectionHandler => _connectionHandler != null;
    public static bool HasHost => _host != null;
    public static bool IsSetup => HasConnectionHandler && HasHost;

    public static void LoadConnectionHandler(Type connectionHandler)
    {
        if (_connectionHandler != null) return;
        if (!connectionHandler.IsAssignableFrom(typeof(DbConnection)))
            throw new Exception("Invalid connection handler, must be DbConnection!");

        _connectionHandler = connectionHandler;
        _log.LogDebug($"Setup connection handler {connectionHandler.Name}");
    }

    public static void LoadHost(string host)
    {
        if (_host != null) return;
        _host = host;
        _log.LogDebug($"Assigned host {host}");
    }

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