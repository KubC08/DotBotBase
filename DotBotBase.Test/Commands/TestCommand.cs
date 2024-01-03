using Discord;
using DotBotBase.Core.Commands;

namespace DotBotBase.Test.Commands;

public class TestCommand : Command
{
    public override string Name => "test";
    public override string Description => "For testing DotBotBase";

    public override ICommandOption[] Options => new ICommandOption[]
    {
        //new CommandOption("testval1", "Simple test value output", ApplicationCommandOptionType.String),
        new CommandOption("testgroup1", "Simple test command group", ApplicationCommandOptionType.SubCommandGroup)
        {
            Options = new ICommandOption[]
            {
                new TestSubCommand()
            }
        }
    };
}