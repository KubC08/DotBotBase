using Discord;
using Discord.WebSocket;
using DotBotBase.Core;
using DotBotBase.Core.Commands;
using DotBotBase.Test.Database;

namespace DotBotBase.Test.Commands;

public class GetTestEntry : Command
{
    public override string Name => "get";
    public override string Description => "Gets a simple test entry from the database";

    public override ICommandOption[] Options => new ICommandOption[]
    {
        new CommandOption("key", "The key of the value to get", ApplicationCommandOptionType.String)
        {
            IsRequired = true
        },
    };

    public override async Task Run(DotBot client, SocketSlashCommand command, Dictionary<string, object> args)
    {
        if (TestModule.TestEntryTable == null) return;
        
        if (!args.TryGetValue("key", out object? key)) return;
        if (key.GetType() != typeof(string)) return;

        TestEntry[] entries = await TestModule.TestEntryTable.GetGeneric(new Dictionary<string, object>()
        {
            { "key", key }
        });
        
        if (entries.Length == 0)
        {
            await command.RespondAsync("Could not find entry with that key!");
            return;
        }

        await command.RespondAsync($"The value associated with the key is: {entries[0].Value}");
    }
}