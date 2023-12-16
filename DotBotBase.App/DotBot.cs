using Discord;
using Discord.WebSocket;
using DotBotBase.App.Config;
using DotBotBase.Core.Logging;

namespace DotBotBase.App;

internal class DotBot : IDisposable
{
    public const string Name = "DotBot";

    public delegate void OnGuildReadyHandler(SocketGuild guild);
    
    public event Action? OnClientReady;
    public event Action? OnClientStop;
    public event OnGuildReadyHandler? OnGuildReady;

    private readonly Logger _log = new Logger(null, Name);
    public readonly DiscordSocketClient Client;
    
    private readonly string _token;
    private readonly bool _autoReconnect;

    public DotBot(BotSettings settings)
    {
        _token = settings.Token ?? "";
        _autoReconnect = settings.AutoReconnect;
        Client = new DiscordSocketClient();
        
        Client.Log += OnDiscordLog;
        Client.Disconnected += OnDiscordDisconnect;
        Client.Ready += OnDiscordReady;
        Client.JoinedGuild += OnDiscordGuildJoin;
    }

    public void Dispose() =>
        Client.Dispose();

    public async Task AddCommand(SlashCommandProperties command) =>
        await Client.Rest.CreateGlobalCommand(command);
    public async Task AddCommand(SocketGuild guild, SlashCommandProperties command) =>
        await Client.Rest.CreateGuildCommand(command, guild.Id);

    public async Task Run()
    {
        await Client.LoginAsync(TokenType.Bot, _token);
        await Client.StartAsync();
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

        while (Client.ConnectionState != ConnectionState.Connected)
        {
            if (Client.ConnectionState == ConnectionState.Connecting)
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
        foreach (SocketGuild guild in Client.Guilds)
            OnGuildReady?.Invoke(guild);
        
        return Task.CompletedTask;
    }
    
    private Task OnDiscordGuildJoin(SocketGuild guild)
    {
        OnGuildReady?.Invoke(guild);
        return Task.CompletedTask;
    }
}