using DotBotBase.Core.Database;
using DotBotBase.Core.Modular;
using DotBotBase.SQLite.Database;

namespace DotBotBase.SQLite;

[ModuleProperties("kubc08.dotbotbase.sqlite")]
public class SQLiteModule : BotModule
{
    public override string Name => "SQLite Extension";
    public override string Version => "1.0.0";
    public override string Author => "KubC08";

    public override void Start() => DatabaseService.LoadConnectionHandler(typeof(SQLiteConnection));
}