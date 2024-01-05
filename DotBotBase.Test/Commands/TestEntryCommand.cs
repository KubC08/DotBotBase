using DotBotBase.Core.Commands;

namespace DotBotBase.Test.Commands;

public class TestEntryCommand : Command
{
    public override string Name => "testentry";
    public override string Description => "Interaction with the test entry";

    public override ICommandOption[] Options => new ICommandOption[]
    {
        new AddTestEntry(),
        new GetTestEntry(),
        new DeleteTestEntry(),
        new UpdateTestEntry()
    };
}