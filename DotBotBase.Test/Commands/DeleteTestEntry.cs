using Discord;
using Discord.WebSocket;
using DotBotBase.Core.Commands;

namespace DotBotBase.Test.Commands;

public class DeleteTestEntry : Command
{
    public override string Name => "delete";
    public override string Description => "Deletes the simple test entry based on key";

    public override ICommandOption[] Options => new ICommandOption[]
    {
        new CommandOption("key", "The key of the entry to delete", ApplicationCommandOptionType.String)
        {
            IsRequired = true
        }
    };

    public override async Task Run(SocketSlashCommand command, Dictionary<string, object> args)
    {
        if (TestModule.TestEntryTable == null) return;
        
        if (!args.TryGetValue("key", out object? key)) return;
        if (key.GetType() != typeof(string)) return;

        await TestModule.TestEntryTable.Delete(new Dictionary<string, object>()
        {
            { "key", key }
        });
        await command.RespondAsync($"The entry associated with key {key} has been deleted!");
    }
}