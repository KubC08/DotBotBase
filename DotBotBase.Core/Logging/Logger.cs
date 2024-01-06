namespace DotBotBase.Core.Logging;

/// <summary>
/// The class responsible for interacting with the "LoggingService" to push logs.
/// </summary>
public class Logger
{
    private string? _category;
    private string _moduleName;
    
    /// <summary>
    /// Create a new instance of the Logger class using a pre-existing Logger.
    /// </summary>
    /// <param name="category">The new category of the given Logger class.</param>
    /// <param name="parent">The pre-existing Logger to copy the information from.</param>
    public Logger(string category, Logger parent)
    {
        _category = category;
        _moduleName = parent._moduleName;
    }

    /// <summary>
    /// Create a new Logger class from scratch.
    /// </summary>
    /// <param name="category">The category of the Logger (optional).</param>
    /// <param name="moduleName">The module name of the Logger.</param>
    public Logger(string? category, string moduleName)
    {
        _category = category;
        _moduleName = moduleName;
    }

    /// <summary>
    /// Duplicate current Logger's information and change the category.
    /// </summary>
    /// <param name="category">The new category name of the duplicate.</param>
    /// <returns>The duplicated instance with the new category name.</returns>
    public Logger CreateCategory(string category) => new Logger(category, this);

    /// <summary>
    /// Send a generic log to the "LoggingService" for processing.
    /// </summary>
    /// <param name="type">The "LogType" of the log to send.</param>
    /// <param name="log">The object to log.</param>
    /// <param name="ex">The attached exception to the log.</param>
    public void Log(LogType type, object? log, Exception? ex = null) =>
        LoggingService.Log(type, BuildLog(log), ex);

    /// <summary>
    /// Send a generic log to the "LoggingService" for processing.
    /// </summary>
    /// <param name="type">The "LogType" of the log to send.</param>
    /// <param name="format">The string format of the log message.</param>
    /// <param name="ex">The attached exception to the log.</param>
    /// <param name="args">The arguments attached to the string format.</param>
    public void Log(LogType type, string format, Exception? ex = null, params object?[] args) =>
        LoggingService.Log(type, BuildLog(string.Format(format, args)), ex);

    /// <summary>
    /// Send a standard "Debug" log type to the "LoggingService" for processing.
    /// </summary>
    /// <param name="log">The object to log.</param>
    public void LogDebug(object? log) => LoggingService.Log(LogType.Debug, BuildLog(log));

    /// <summary>
    /// Send a standard "Debug" log type to the "LoggingService" for processing.
    /// </summary>
    /// <param name="format">The string format of the log message.</param>
    /// <param name="args">The arguments attached to the string format.</param>
    public void LogDebug(string format, params object?[] args) =>
        LoggingService.Log(LogType.Debug, BuildLog(string.Format(format, args)));

    /// <summary>
    /// Send a standard "Info" log type to the "LoggingService" for processing.
    /// </summary>
    /// <param name="log">The object to log.</param>
    public void LogInfo(object? log) => LoggingService.Log(LogType.Info, BuildLog(log));

    /// <summary>
    /// Send a standard "Info" log type to the "LoggingService" for processing.
    /// </summary>
    /// <param name="format">The string format of the log message.</param>
    /// <param name="args">The arguments attached to the string format.</param>
    public void LogInfo(string format, params object?[] args) =>
        LoggingService.Log(LogType.Info, BuildLog(string.Format(format, args)));

    /// <summary>
    /// Send a standard "Warning" log type to the "LoggingService" for processing.
    /// </summary>
    /// <param name="log">The object to log.</param>
    public void LogWarning(object? log) => LoggingService.Log(LogType.Warning, BuildLog(log));

    /// <summary>
    /// Send a standard "Warning" log type to the "LoggingService" for processing.
    /// </summary>
    /// <param name="format">The string format of the log message.</param>
    /// <param name="args">The arguments attached to the string format.</param>
    public void LogWarning(string format, params object?[] args) =>
        LoggingService.Log(LogType.Warning, BuildLog(string.Format(format, args)));

    /// <summary>
    /// Send a standard "Error" log type with an exception to the "LoggingService" for processing.
    /// </summary>
    /// <param name="log">The object to log.</param>
    /// <param name="ex">The attached exception to the log.</param>
    public void LogError(object? log, Exception ex) => LoggingService.Log(LogType.Error, BuildLog(log), ex);

    /// <summary>
    /// Send a standard "Error" log type with an exception to the "LoggingService" for processing.
    /// </summary>
    /// <param name="format">The string format of the log message.</param>
    /// <param name="ex">The attached exception to the log.</param>
    /// <param name="args">The arguments attached to the string format.</param>
    public void LogError(string format, Exception ex, params object?[] args) =>
        LoggingService.Log(LogType.Error, BuildLog(string.Format(format, args)), ex);
    
    private string BuildLog(object? log)
    {
        string logText = $"[{_moduleName}]";
        if (_category != null) logText += $"[{_category}]";
        logText += $" {log}";

        return logText;
    }
}