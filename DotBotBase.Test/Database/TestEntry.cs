using DotBotBase.Core.Database;

namespace DotBotBase.Test.Database;

public class TestEntry
{
    [DbColumn("key", PrimaryKey = true)] public string Key = "";

    [DbColumn("value", NotNullable = true)] public string Value = "";
}