namespace DotBotBase.Core.Logging;

[Flags]
public enum LogType : ushort
{
    Debug = 1,
    Info = 2,
    Warning = 4,
    Error = 8
}