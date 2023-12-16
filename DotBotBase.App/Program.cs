using DotBotBase.App;
using DotBotBase.App.Config;
using DotBotBase.Core.Modular;

if (!Directory.Exists("config")) Directory.CreateDirectory("config");
if (!Directory.Exists("modules")) Directory.CreateDirectory("modules");
if (!Directory.Exists("libraries")) Directory.CreateDirectory("libraries");

BotSettings settings = ConfigService.GetOrSetConfig<BotSettings>("config/settings.json");
if (string.IsNullOrEmpty(settings.Token)) return;

DotBot bot = new DotBot(settings);

bot.OnClientReady += ModuleService.StartModules;
bot.OnClientStop += ModuleService.ShutdownModules;

Task running = bot.Run();
running.Wait();
Console.ReadLine();

bot.Dispose();