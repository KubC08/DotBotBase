using Discord;
using Discord.WebSocket;
using DotBotBase.App.Config;
using DotBotBase.Core.Logging;

namespace DotBotBase.App;

internal class DotBot : IDisposable
{
    public const string Name = "DotBot";
    
    public event Action? OnClientReady;
    public event Action? OnClientStop;

    private readonly Logger _log = new Logger(null, Name);
    private readonly DiscordSocketClient _client;
    
    private readonly string _token;
    private readonly bool _autoReconnect;

    public DotBot(BotSettings settings)
    {
        _token = settings.Token ?? "";
        _autoReconnect = settings.AutoReconnect;
        _client = new DiscordSocketClient();
        
        _client.Log += OnDiscordLog;
        _client.Disconnected += OnDiscordDisconnect;
        _client.Ready += OnDiscordReady;
    }

    public void Dispose() =>
        _client.Dispose();

    public async Task Run()
    {
        await _client.LoginAsync(TokenType.Bot, _token);
        await _client.StartAsync();
    }
    
    private Task OnDiscordLog(LogMessage arg)
    {
        LogType? logType = null;
        Exception? ex = null;
        switch (arg.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                logType = LogType.Error;
                ex = arg.Exception;
                break;
            case LogSeverity.Warning:
                logType = LogType.Warning;
                break;
            case LogSeverity.Info:
                logType = LogType.Info;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                logType = LogType.Debug;
                break;
        }
        if (logType == null) return Task.CompletedTask;
        
        _log.Log((LogType)logType, arg.Message, ex);
        return Task.CompletedTask;
    }
    
    private async Task OnDiscordDisconnect(Exception? arg)
    {
        if (!_autoReconnect)
        {
            OnClientStop?.Invoke();
            return;
        }

        while (_client.ConnectionState != ConnectionState.Connected)
        {
            if (_client.ConnectionState == ConnectionState.Connecting)
            {
                await Task.Delay(1000);
                continue;
            }
            
            await Run();
            await Task.Delay(5000);
        }
    }
    
    private Task OnDiscordReady()
    {
        OnClientReady?.Invoke();
        return Task.CompletedTask;
    }
}