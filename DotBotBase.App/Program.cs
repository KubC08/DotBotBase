using Discord;
using DotBotBase.App;
using DotBotBase.App.Config;
using DotBotBase.Core.Commands;
using DotBotBase.Core.Config;
using DotBotBase.Core.Database;
using DotBotBase.Core.Logging;
using DotBotBase.Core.Modular;

string configDirectory = Path.Join(Directory.GetCurrentDirectory(), "config");
string modulesDirectory = Path.Join(Directory.GetCurrentDirectory(), "modules");
string librariesDirectory = Path.Join(Directory.GetCurrentDirectory(), "libraries");

if (!Directory.Exists(configDirectory)) Directory.CreateDirectory(configDirectory);
if (!Directory.Exists(modulesDirectory)) Directory.CreateDirectory(modulesDirectory);
if (!Directory.Exists(librariesDirectory)) Directory.CreateDirectory(librariesDirectory);

LoggingService.AllowedTypes = LoggingService.All;

ConfigService.Setup(configDirectory);
BotSettings? settings = ConfigService.GetOrSetConfig<BotSettings>("dotbotbase");
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

bot.Dispose();