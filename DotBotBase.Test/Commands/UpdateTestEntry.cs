using Discord;
using Discord.WebSocket;
using DotBotBase.Core;
using DotBotBase.Core.Commands;
using DotBotBase.Test.Database;

namespace DotBotBase.Test.Commands;

public class UpdateTestEntry : Command
{
    public override string Name => "update";
    public override string Description => "Update the value of a given key";

    public override ICommandOption[] Options => new ICommandOption[]
    {
        new CommandOption("key", "The key of the entry to update", ApplicationCommandOptionType.String)
        {
            IsRequired = true
        },
        new CommandOption("value", "The new value to set the entry to", ApplicationCommandOptionType.String)
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

        await TestModule.TestEntryTable.Update(new Dictionary<string, object>()
        {
            { "key", key }
        }, new TestEntry()
        {
            Key = (string)key,
            Value = (string)value
        });
        await command.RespondAsync($"Successfully updated entry for key {key}");
    }
}