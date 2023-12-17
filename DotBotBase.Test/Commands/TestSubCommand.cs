using Discord;
using Discord.WebSocket;
using DotBotBase.Core.Commands;

namespace DotBotBase.Test.Commands;

public class TestSubCommand : Command
{
    public override string? Name => "testsubcommand";
    public override string? Description => "Simple test sub command";

    public override ICommandOption[] Options => new ICommandOption[]
    {
        new CommandOption("testval2", "Simple test val 2", ApplicationCommandOptionType.String)
        {
            IsRequired = true
        }
    };

    public override async Task Run(SocketSlashCommand command, object? argument)
    {
        await command.RespondAsync($"Your sub command argument is {argument}");
    }
}