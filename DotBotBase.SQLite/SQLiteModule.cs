using DotBotBase.Core.Config;
using DotBotBase.Core.Modular;
using DotBotBase.SQLite.Config;
using Microsoft.Data.Sqlite;

namespace DotBotBase.SQLite;

[ModuleProperties("kubc08.dotbotbase.sqlite")]
public class SQLiteModule : BotModule
{
    public static ModuleSettings? Settings { get; private set; } = null;
    public static SqliteConnection? Connection { get; private set; } = null;
    
    public override string Name => "SQLite Extension";
    public override string Version => "1.0.0";
    public override string Author => "KubC08";

    public override void Start()
    {
        Settings = ConfigService.GetOrSetConfig<ModuleSettings>("sqlite");
        if (Settings == null || string.IsNullOrEmpty(Settings.FilePath)) return;
        
        Connection = new SqliteConnection($"Data Source={Settings.FilePath};Version=3;New=True;Compress=True;");
    }
}