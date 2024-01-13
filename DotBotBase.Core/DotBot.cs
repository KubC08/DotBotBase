using System.Collections.ObjectModel;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DotBotBase.Core.Commands;

namespace DotBotBase.Core;

/// <summary>
/// Handles the actual Discord client and bot.
/// </summary>
public class DotBot : IDisposable
{
    internal const string Name = "DotBot Core";
    
    public event Func<DiscordSocketClient, Task>? OnClientReady;
    public event Func<DiscordSocketClient, Task>? OnClientConnect;
    public event Func<DiscordSocketClient, Exception, Task>? OnClientDisconnect;
    public event Func<LogMessage, Task>? OnLog;
    public event Func<SocketSlashCommand, Task>? OnSlashCommandExecute;
    public event Func<SocketGuild, Task>? OnGuildReady;

    /// <summary>
    /// The Discord bot's token.
    /// </summary>
    public string Token { get; private set; }
    /// <summary>
    /// Does the bot have sharding enabled.
    /// </summary>
    public bool IsSharded { get; private set; }

    /// <summary>
    /// The Discord socket client (null if "IsSharded" is enabled).
    /// </summary>
    public DiscordSocketClient? SocketClient { get; private set; }
    /// <summary>
    /// The Discord sharded client (null if "IsSharded" is disabled).
    /// </summary>
    public DiscordShardedClient? ShardedClient { get; private set; }

    /// <summary>
    /// Returns either the sharded or socket client based on "IsSharded". If null then something went wrong.
    /// </summary>
    public BaseSocketClient? Client => (IsSharded ? ShardedClient : SocketClient);

    /// <summary>
    /// Create and initialize the bot framework, as well as the Discord client.
    /// </summary>
    /// <param name="token">The token to access the bot.</param>
    /// <param name="isSharded">Is the bot sharded.</param>
    public DotBot(string token, bool isSharded)
    {
        Token = token;
        IsSharded = isSharded;

        if (isSharded) ShardedClient = new DiscordShardedClient();
        else SocketClient = new DiscordSocketClient();

        if (Client != null)
        {
            Client.Log += message => OnLog?.Invoke(message);
            Client.SlashCommandExecuted += command => OnSlashCommandExecute?.Invoke(command);
            Client.JoinedGuild += guild => OnGuildReady?.Invoke(guild);

            if (SocketClient != null)
            {
                SocketClient.Ready += () => OnClientReady?.Invoke(SocketClient);
                SocketClient.Connected += () => OnClientConnect?.Invoke(SocketClient);
                SocketClient.Disconnected += exception => OnClientDisconnect?.Invoke(SocketClient, exception);
            }
            
            if (ShardedClient != null)
            {
                ShardedClient.ShardReady += client => OnClientReady?.Invoke(client);
                ShardedClient.ShardConnected += client => OnClientConnect?.Invoke(client);
                ShardedClient.ShardDisconnected += (exception, client) => OnClientDisconnect?.Invoke(client, exception);
            }

            OnClientReady += async client =>
            {
                foreach (SocketGuild guild in client.Guilds)
                    await OnGuildReady?.Invoke(guild)!;
            };
        }
    }
    public void Dispose() =>
        Client?.Dispose();
    
    /// <summary>
    /// Starts the bot.
    /// </summary>
    public async Task Run()
    {
        if (Client == null) return;

        await Client.LoginAsync(TokenType.Bot, Token);
        await Client.StartAsync();
    }
    
    /// <summary>
    /// Adds Discord slash command to all the guilds.
    /// </summary>
    /// <param name="command">The Discord slash command to add.</param>
    /// <returns>The created global command or null if failed to create.</returns>
    public async Task<RestGlobalCommand?> AddGlobalCommand(SlashCommandProperties command)
    {
        if (Client == null) return null;
        return await Client.Rest.CreateGlobalCommand(command);
    }
    
    /// <summary>
    /// Adds Discord slash command to all guilds.
    /// </summary>
    /// <param name="command">The Discord slash command to add.</param>
    /// <returns>The created global command or null if failed to create.</returns>
    public async Task<RestGlobalCommand?> AddGlobalCommand(Command command)
    {
        if (Client == null) return null;
        
        SlashCommandProperties properties = CommandService.Build(command);
        return await Client.Rest.CreateGlobalCommand(properties);
    }

    /// <summary>
    /// Gets all the global commands that the bot has created.
    /// </summary>
    /// <returns>The list of global commands.</returns>
    public async Task<IReadOnlyCollection<RestGlobalCommand>?> GetAllGlobalCommands()
    {
        if (Client == null) return null;
        return await Client.Rest.GetGlobalApplicationCommands();
    }

    /// <summary>
    /// Adds a Discord slash command to a specified guild.
    /// </summary>
    /// <param name="guild">The guild to add the command to.</param>
    /// <param name="command">The Discord slash command to add.</param>
    /// <returns>The created guild command or null if failed to create.</returns>
    public async Task<RestGuildCommand?> AddGuildCommand(SocketGuild guild, SlashCommandProperties command)
    {
        if (Client == null) return null;
        return await Client.Rest.CreateGuildCommand(command, guild.Id);
    }

    /// <summary>
    /// Gets all the commands for every guild the bot is in.
    /// </summary>
    /// <returns>The dictionary of guild id and list of commands attached to it.</returns>
    public async Task<ReadOnlyDictionary<ulong, IReadOnlyCollection<RestGuildCommand>>?> GetAllGuildCommands()
    {
        if (Client == null) return null;
        
        Dictionary<ulong, IReadOnlyCollection<RestGuildCommand>> guildCommands = new Dictionary<ulong, IReadOnlyCollection<RestGuildCommand>>();
        foreach (var guild in Client.Guilds)
        {
            if (guild != null)
                guildCommands.TryAdd(guild.Id, await Client.Rest.GetGuildApplicationCommands(guild.Id));
        }

        return new ReadOnlyDictionary<ulong, IReadOnlyCollection<RestGuildCommand>>(guildCommands);
    }

    /// <summary>
    /// Adds a Discord slash command to a specified guild.
    /// </summary>
    /// <param name="guild">The guild to add the command to.</param>
    /// <param name="command">The Discord slash command to add.</param>
    /// <returns>The created guild command or null if failed to create.</returns>
    public async Task<RestGuildCommand?> AddGuildCommand(SocketGuild guild, Command command)
    {
        if (Client == null) return null;

        SlashCommandProperties properties = CommandService.Build(command);
        return await Client.Rest.CreateGuildCommand(properties, guild.Id);
    }
}