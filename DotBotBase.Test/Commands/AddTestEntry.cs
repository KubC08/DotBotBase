using Discord;
using Discord.WebSocket;
using DotBotBase.Core.Commands;

namespace DotBotBase.Test.Commands;

public class AddTestEntry : Command
{
    public override string Name => "add";
    public override string Description => "Adds a simple test entry to database";

    public override ICommandOption[] Options => new ICommandOption[]
    {
        new CommandOption("key", "The key of the value to add", ApplicationCommandOptionType.String),
        new CommandOption("value","The value to add", ApplicationCommandOptionType.String)
    };

    public override async Task Run(SocketSlashCommand command, object? argument)
    {
        if (argument == null || argument.GetType() != typeof(string)) return;
    }
}