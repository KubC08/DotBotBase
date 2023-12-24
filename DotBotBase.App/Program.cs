using Discord;
using DotBotBase.App;
using DotBotBase.App.Config;
using DotBotBase.Core.Commands;
using DotBotBase.Core.Logging;
using DotBotBase.Core.Modular;

if (!Directory.Exists("config")) Directory.CreateDirectory("config");
if (!Directory.Exists("modules")) Directory.CreateDirectory("modules");
//if (!Directory.Exists("libraries")) Directory.CreateDirectory("libraries");

LoggingService.AllowedTypes = LoggingService.All;

BotSettings? settings = ConfigService.GetOrSetConfig<BotSettings>("config/settings.json");
if (settings == null || string.IsNullOrEmpty(settings.Token)) return;

DotBot bot = new DotBot(settings);

ModuleService.LoadModules(Path.Combine(Directory.GetCurrentDirectory(), "modules"));
/*foreach (string library in Directory.GetFiles("libraries", "*.dll"))
    ModuleService.LoadLibrary(Path.Combine(Directory.GetCurrentDirectory(), library));
foreach (string module in Directory.GetFiles("modules", "*.dll"))
    ModuleService.LoadModule(Path.Combine(Directory.GetCurrentDirectory(), module));*/

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