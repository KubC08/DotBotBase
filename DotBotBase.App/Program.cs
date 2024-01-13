using Discord;
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

string configDirectory = Path.Join(Directory.GetCurrentDirectory(), "config");
string modulesDirectory = Path.Join(Directory.GetCurrentDirectory(), "modules");

if (!Directory.Exists(configDirectory)) Directory.CreateDirectory(configDirectory);
if (!Directory.Exists(modulesDirectory)) Directory.CreateDirectory(modulesDirectory);

string? envEnableDebug = Environment.GetEnvironmentVariable("ENABLE_DEBUG");
if (envEnableDebug != null && envEnableDebug.ToLower() == "true")
    LoggingService.AllowedTypes = LoggingService.All;

log.LogInfo("Initializing configuration and settings...");
ConfigService.Setup(configDirectory);
BotSettings? settings = ConfigService.GetOrSetConfig<BotSettings>("settings");
if (settings == null || string.IsNullOrEmpty(settings.Token)) return;
if (settings.ShowDebugLogs) LoggingService.AllowedTypes = LoggingService.All;
log.LogInfo("Successfully initialized configuration and settings!");

log.LogInfo("Initializing DotBot and Discord...");
DotBot bot = new DotBot(settings.Token, settings.IsSharded);
log.LogInfo("Successfully initialized DotBot and Discord!");
log.LogInfo("Initializing database system if present...");
if (!string.IsNullOrEmpty(settings.DatabaseHost)) DatabaseService.LoadHost(settings.DatabaseHost);
log.LogInfo("Successfully initialized database system!");

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
    if (!settings.AutoReconnect || settings.IsSharded || bot.SocketClient == null) return;
    
    while (bot.SocketClient.ConnectionState != ConnectionState.Connected)
    {
        if (bot.SocketClient.ConnectionState == ConnectionState.Connecting)
        {
            await Task.Delay(1000);
            continue;
        }
            
        await bot.Run();
        await Task.Delay(5000);
    }
};

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