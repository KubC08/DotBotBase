using DotBotBase.Core.Modular;
using DotBotBase.Test.Commands;

namespace DotBotBase.Test;

public class TestModule : BotModule
{
    public override string Name => "Test Module";
    public override string Version => "1.0.0";
    public override string Author => "KubC08";

    public override void Start()
    {
        AddCommand<TestCommand>();
    }
}