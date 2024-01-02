using DotBotBase.Core.Database;

namespace DotBotBase.Test.Database;

public class TestEntry
{
    [DbColumn("key")]
    public string Key;

    [DbColumn("value")]
    public string Value;
}