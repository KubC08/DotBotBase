using Discord;
using Discord.WebSocket;
using DotBotBase.Core;
using DotBotBase.Core.Commands;
using DotBotBase.Test.Database;

namespace DotBotBase.Test.Commands;

public class AddTestEntry : Command
{
    public override string Name => "add";
    public override string Description => "Adds a simple test entry to database";

    public override ICommandOption[] Options => new ICommandOption[]
    {
        new CommandOption("key", "The key of the value to add", ApplicationCommandOptionType.String)
        {
            IsRequired = true
        },
        new CommandOption("value","The value to add", ApplicationCommandOptionType.String)
        {
            IsRequired = true
        }
    };

    public override async Task Run(DotBot client, SocketSlashCommand command, Dictionary<string, object> args)
    {
        if (TestModule.TestEntryTable == null) return;
        
        if (!args.TryGetValue("key", out object? key)) return;
        if (!args.TryGetValue("value", out object? value)) return;
        if (key.GetType() != typeof(string) || value.GetType() != typeof(string)) return;

        await TestModule.TestEntryTable.Create(new TestEntry()
        {
            Key = (string)key,
            Value = (string)value
        });
        await command.RespondAsync($"Successfully stored {key} into DB!");
    }
}