using Discord;

namespace DotBotBase.Core.Commands;

public interface ICommandOption
{
    public string? Name { get; }
    public string? Description { get; }
    public ApplicationCommandOptionType? Type { get; }

    public bool IsRequired { get; }
    public bool IsAutoComplete { get; }
    public int? MaxLength { get; }
    public int? MinLength { get; }
    public double? MaxValue { get; }
    public double? MinValue { get; }

    public ApplicationCommandOptionChoiceProperties[] Choices { get; }
    public ChannelType[] ChannelTypes { get; }

    public ICommandOption[] Options { get; }
}