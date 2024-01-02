using Discord;
using Discord.WebSocket;
using DotBotBase.Core.Commands;

namespace DotBotBase.Test.Commands;

public class GetTestEntry : Command
{
    public override string Name => "get";
    public override string Description => "Gets a simple test entry from the database";

    public override ICommandOption[] Options => new ICommandOption[]
    {
        new CommandOption("key", "The key of the value to get", ApplicationCommandOptionType.String),
    };

    public override async Task Run(SocketSlashCommand command, object? argument)
    {
    }
}