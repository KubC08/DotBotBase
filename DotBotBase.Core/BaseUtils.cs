using DotBotBase.Core.Logging;

namespace DotBotBase.Core;

public static class BaseUtils
{
    public static void SafeInvoke(this Logger log, string error, Action func)
    {
        try
        {
            func();
        }
        catch (Exception ex)
        {
            log.LogError(error, ex);
        }
    }
}