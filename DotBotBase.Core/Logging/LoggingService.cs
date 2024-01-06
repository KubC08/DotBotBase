namespace DotBotBase.Core.Logging;

/// <summary>
/// The service responsible for all the logging done in the program.
/// </summary>
public static class LoggingService
{
    private const ConsoleColor DebugColor = ConsoleColor.Gray;
    private const ConsoleColor DefaultColor = ConsoleColor.White;
    private const ConsoleColor WarningColor = ConsoleColor.DarkYellow;
    private const ConsoleColor ErrorColor = ConsoleColor.Red;

    /// <summary>
    /// The "all" logging filter.
    /// </summary>
    public const LogType All = LogType.Debug | LogType.Error | LogType.Info | LogType.Warning;
    
    public delegate void OnLogHandler(LogType type, string log, Exception? ex = null);
    /// <summary>
    /// Called when a log should be processed.
    /// </summary>
    public static event OnLogHandler? OnLog;
    
    /// <summary>
    /// The standard or "default" logging filter.
    /// </summary>
    public static LogType AllowedTypes = LogType.Error | LogType.Warning | LogType.Info;

    internal static void Log(LogType type, string log, Exception? ex = null)
    {
        if (!AllowedTypes.HasFlag(type)) return;
        
        OnLog?.Invoke(type, log, ex);
        if (type == LogType.Debug)
            SetConsoleColor(DebugColor, () => Console.WriteLine(log));
        else if (type == LogType.Info)
            SetConsoleColor(DefaultColor, () => Console.WriteLine(log));
        else if (type == LogType.Warning)
            SetConsoleColor(WarningColor, () => Console.WriteLine(log));
        else if (type == LogType.Error)
        {
            SetConsoleColor(ErrorColor, () =>
            {
                Console.WriteLine(log);
                Console.WriteLine(ex);
            });
        }
    }
    
    private static void SetConsoleColor(ConsoleColor color, Action func)
    {
        ConsoleColor previousColor = Console.ForegroundColor;

        Console.ForegroundColor = color;
        func();
        Console.ForegroundColor = previousColor;
    }
}