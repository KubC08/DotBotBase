using Discord.WebSocket;
using DotBotBase.Core.Commands;

namespace DotBotBase.Test.Commands;

public class TestCommand : Command
{
    public override string Name => "test";
    public override string Description => "For testing DotBotBase";

    public override Task Run(SocketSlashCommand command, object? argument)
    {
    }
}