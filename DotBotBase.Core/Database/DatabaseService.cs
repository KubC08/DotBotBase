using DotBotBase.Core.Logging;

namespace DotBotBase.Core.Database;

public static class DatabaseService
{
    private static readonly Logger _log = new Logger("Database Service", DotBotInfo.Name);

    private static Type? _connectionHandler;

    public static void LoadConnectionHandler(Type connectionHandler)
    {
        if (_connectionHandler != null) return;
        if (!connectionHandler.IsAssignableFrom(typeof(DbConnection)))
            throw new Exception("Invalid connection handler, must be DbConnection!");

        _connectionHandler = connectionHandler;
        _log.LogDebug($"Setup connection handler {connectionHandler.Name}");
    }

    public static DbConnection Connect(string host, string database)
    {
        if (_connectionHandler == null)
            throw new Exception("No connection handler loaded!");
        
        DbConnection? connection = (DbConnection?)Activator.CreateInstance(_connectionHandler);
        if (connection == null)
            throw new Exception("Could not create connection handler!");
        
        connection.Connect(host, database);
        _log.LogInfo($"Established connection using {_connectionHandler.Name} to {host}");
        return connection;
    }
}