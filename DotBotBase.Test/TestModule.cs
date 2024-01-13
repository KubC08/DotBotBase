using DotBotBase.Core.Database;
using DotBotBase.Core.Modular;
using DotBotBase.Test.Commands;
using DotBotBase.Test.Database;

namespace DotBotBase.Test;

[ModuleProperties("kubc08.dotbotbase.test")]
public class TestModule : BotModule
{
    public static DbConnection? Database;
    public static DbTable<TestEntry>? TestEntryTable;
    
    public override string Name => "Test Module";
    public override string Version => "1.0.0";
    public override string Author => "KubC08";

    public override async Task StartAsync()
    {
        LoadGlobalCommand<TestCommand>();
        
        if (DatabaseService.IsSetup)
        {
            Database = DatabaseService.Connect("test");
            TestEntryTable = await Database.GetOrCreateTable<TestEntry>("test_entry");
            
            LoadGlobalCommand<TestEntryCommand>();
        }
    }
}