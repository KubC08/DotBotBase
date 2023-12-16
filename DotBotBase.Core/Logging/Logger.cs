namespace DotBotBase.Core.Logging;

public class Logger
{
    private string? _category;
    private string _moduleName;
    
    public Logger(string category, Logger parent)
    {
        _category = category;
        _moduleName = parent._moduleName;
    }

    public Logger(string? category, string moduleName)
    {
        _category = category;
        _moduleName = moduleName;
    }

    public Logger CreateCategory(string category) => new Logger(category, this);

    public void Log(LogType type, object? log, Exception? ex = null) =>
        LoggingService.Log(type, BuildLog(log), ex);

    public void Log(LogType type, string format, Exception? ex = null, params object?[] args) =>
        LoggingService.Log(type, BuildLog(string.Format(format, args)), ex);

    public void LogDebug(object? log) => LoggingService.Log(LogType.Debug, BuildLog(log));

    public void LogDebug(string format, params object?[] args) =>
        LoggingService.Log(LogType.Debug, BuildLog(string.Format(format, args)));

    public void LogInfo(object? log) => LoggingService.Log(LogType.Info, BuildLog(log));

    public void LogInfo(string format, params object?[] args) =>
        LoggingService.Log(LogType.Info, BuildLog(string.Format(format, args)));

    public void LogWarning(object? log) => LoggingService.Log(LogType.Warning, BuildLog(log));

    public void LogWarning(string format, params object?[] args) =>
        LoggingService.Log(LogType.Warning, BuildLog(string.Format(format, args)));

    public void LogError(object? log, Exception ex) => LoggingService.Log(LogType.Error, BuildLog(log), ex);

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