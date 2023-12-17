using Discord;

namespace DotBotBase.Core.Commands;

public class CommandOption : CommandOptionExtendable
{
    public override string Name { get; }
    public override string Description { get; }
    public override ApplicationCommandOptionType Type { get; }
}