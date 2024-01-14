using Discord;
using Discord.Rest;
using DotBotBase.App.Config;
using DotBotBase.Core;
using DotBotBase.Core.Commands;
using DotBotBase.Core.Config;
using DotBotBase.Core.Database;
using DotBotBase.Core.Logging;
using DotBotBase.Core.Modular;

const string name = "DotBot";
Logger log = new Logger(null, name);
Logger discordLog = new Logger("Discord", name);

Dictionary<string, RestGlobalCommand> globalDiscordCommands = new Dictionary<string, RestGlobalCommand>();
Dictionary<ulong, Dictionary<string, RestGuildCommand>> guildDiscordCommands = new Dictionary<ulong, Dictionary<string, RestGuildCommand>>();

bool isReady = false;
List<Command> preloadedGlobalCommands = new List<Command>();
Dictionary<ulong, List<Command>> preloadedGuildCommands = new Dictionary<ulong, List<Command>>();

string configDirectory = Path.Join(Directory.GetCurrentDirectory(), "config");
string modulesDirectory = Path.Join(Directory.GetCurrentDirectory(), "modules");

if (!Directory.Exists(configDirectory)) Directory.CreateDirectory(configDirectory);
if (!Directory.Exists(modulesDirectory)) Directory.CreateDirectory(modulesDirectory);

string? envEnableDebug = Environment.GetEnvironmentVariable("ENABLE_DEBUG");
string? envRebuildCommands = Environment.GetEnvironmentVariable("REBUILD_COMMANDS");
string? envModifyCommands = Environment.GetEnvironmentVariable("MODIFY_COMMANDS");

if (envEnableDebug != null && envEnableDebug.ToLower() == "true")
    LoggingService.AllowedTypes = LoggingService.All;

log.LogInfo("Initializing configuration and settings...");
ConfigService.Setup(configDirectory);
BotSettings? settings = ConfigService.GetOrSetConfig<BotSettings>("settings");
if (settings == null || string.IsNullOrEmpty(settings.Token)) return;
if (settings.ShowDebugLogs) LoggingService.AllowedTypes = LoggingService.All;
log.LogInfo("Successfully initialized configuration and settings!");

bool shouldRebuildCommands = (envRebuildCommands != null && envRebuildCommands.ToLower() == "true") ||
                             settings.AlwaysRebuildCommands;
bool shouldModifyCommands = (envModifyCommands != null && envModifyCommands.ToLower() == "true") ||
                            settings.ModifyExistingCommands;

log.LogInfo("Initializing DotBot and Discord...");
DotBot bot = new DotBot(settings.Token);
log.LogInfo("Successfully initialized DotBot and Discord!");
log.LogInfo("Initializing database system if present...");
if (!string.IsNullOrEmpty(settings.DatabaseHost)) DatabaseService.LoadHost(settings.DatabaseHost);
log.LogInfo("Successfully initialized database system!");

CommandService.OnGlobalCommandLoad += async command =>
{
    if (command.Name == null) return;
    if (!isReady)
    {
        preloadedGlobalCommands.Add(command);
        return;
    }

    RestGlobalCommand? discordCommand = await bot?.AddGlobalCommand(command)!;
    if (discordCommand == null) return;
    
    globalDiscordCommands.Add(command.Name, discordCommand);
};

CommandService.OnGuildCommandLoad += async (command, guildId) =>
{
    if (command.Name == null) return;
    if (!isReady)
    {
        if (!preloadedGuildCommands.TryGetValue(guildId, out var commands))
        {
            commands = new List<Command>();
            preloadedGuildCommands.Add(guildId, commands);
        }
        commands.Add(command);
        return;
    }

    RestGuildCommand? discordCommand = await bot?.AddGuildCommand(guildId, command)!;
    if (discordCommand == null) return;

    if (!guildDiscordCommands.TryGetValue(guildId, out var guildCommands))
    {
        guildCommands = new Dictionary<string, RestGuildCommand>();
        guildDiscordCommands.Add(guildId, guildCommands);
    }
    guildCommands.Add(command.Name, discordCommand);
};

log.LogInfo("Setting up modules and module service...");
AppDomain.CurrentDomain.AssemblyResolve += ModuleService.ResolveLibrary;
ModuleService.SetupLibraries(modulesDirectory);
ModuleService.LoadModules(bot, modulesDirectory);
log.LogInfo("Successfully setup all modules and module services!");

bot.OnLog += message =>
{
    LogType? logType = null;
    Exception? ex = null;
    switch (message.Severity)
    {
        case LogSeverity.Critical:
        case LogSeverity.Error:
            logType = LogType.Error;
            ex = message.Exception;
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
        
    discordLog.Log((LogType)logType, message.Message, ex);
    return Task.CompletedTask;
};

bot.OnClientDisconnect += async (client, exception) =>
{
    log.LogDebug($"Client disconnected {client.ShardId}");
    if (!settings.AutoReconnect || bot.SocketClient == null) return;
    
    while (bot.SocketClient.ConnectionState != ConnectionState.Connected)
    {
        if (bot.SocketClient.ConnectionState == ConnectionState.Connecting)
        {
            await Task.Delay(1000);
            continue;
        }
            
        await bot.Run();
        log.LogDebug($"Reconnecting client...");
        await Task.Delay(5000);
    }
};

bot.OnClientReady += async client =>
{
    isReady = true;
    if (shouldRebuildCommands)
    {
        List<ApplicationCommandProperties> commands = new List<ApplicationCommandProperties>();
        foreach (var command in preloadedGlobalCommands)
            commands.Add(CommandService.Build(command));
        await client.BulkOverwriteGlobalApplicationCommandsAsync(commands.ToArray());
        log.LogDebug($"Rebuilt {commands.Count} global commands for Discord");
    }
    else if (shouldModifyCommands)
    {
        var discordCommands = await bot.GetAllGlobalCommands();
        if (discordCommands != null)
            foreach (var command in discordCommands)
                globalDiscordCommands.Add(command.Name, command);
        
        foreach (var command in preloadedGlobalCommands)
        {
        }
    }
};

bot.OnGuildReady += async guild =>
{
    if (shouldRebuildCommands)
    {
        if (preloadedGuildCommands.TryGetValue(guild.Id, out var guildCommands))
        {
            List<ApplicationCommandProperties> commands = new List<ApplicationCommandProperties>();
            foreach (var command in guildCommands)
                commands.Add(CommandService.Build(command));
            await guild.BulkOverwriteApplicationCommandAsync(commands.ToArray());
            log.LogDebug($"Rebuilt {commands.Count} guild commands for guild {guild.Id}");
        }
    }
    else if (shouldModifyCommands)
    {
        if (!guildDiscordCommands.TryGetValue(guild.Id, out var guildCommands))
        {
            guildCommands = new Dictionary<string, RestGuildCommand>();
            guildDiscordCommands.Add(guild.Id, guildCommands);
        }

        var discordCommands = await guild.GetAllCommands(bot);
        if (discordCommands != null)
            foreach (var command in discordCommands)
                guildCommands.Add(command.Name, command);
    }
};

bot.OnSlashCommandExecute += command => CommandService.RunCommand(bot, command);

/*string configDirectory = Path.Join(Directory.GetCurrentDirectory(), "config");
string modulesDirectory = Path.Join(Directory.GetCurrentDirectory(), "modules");
string librariesDirectory = Path.Join(Directory.GetCurrentDirectory(), "libraries");

if (!Directory.Exists(configDirectory)) Directory.CreateDirectory(configDirectory);
if (!Directory.Exists(modulesDirectory)) Directory.CreateDirectory(modulesDirectory);
if (!Directory.Exists(librariesDirectory)) Directory.CreateDirectory(librariesDirectory);

LoggingService.AllowedTypes = LoggingService.All;

ConfigService.Setup(configDirectory);
BotSettings? settings = ConfigService.GetOrSetConfig<BotSettings>("settings");
if (settings == null || string.IsNullOrEmpty(settings.Token)) return;

DotBot bot = new DotBot(settings);
if (!string.IsNullOrEmpty(settings.DatabaseHost)) DatabaseService.LoadHost(settings.DatabaseHost);

AppDomain.CurrentDomain.AssemblyResolve += ModuleService.ResolveLibrary;
ModuleService.SetupLibraries(librariesDirectory);
ModuleService.LoadModules(modulesDirectory);

bot.OnClientReady += async () =>
{
    bot.Client.BulkOverwriteGlobalApplicationCommandsAsync(Array.Empty<ApplicationCommandProperties>());
    ModuleService.StartModules();
    foreach (Command command in CommandService.Commands)
        await bot.AddCommand(CommandService.Build(command));
};
bot.OnClientStop += ModuleService.ShutdownModules;
bot.OnCommand += CommandService.RunCommand;

Task running = bot.Run();
running.Wait();
Console.ReadLine();

bot.Dispose();*/