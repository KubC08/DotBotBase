using Discord;

namespace DotBotBase.Core.Commands;

public class CommandOption : ICommandOption
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ApplicationCommandOptionType? Type { get; set; }

    public bool IsRequired { get; set; } = false;
    public bool IsAutoComplete { get; set; } = false;
    public int? MaxLength { get; set; } = null;
    public int? MinLength { get; set; } = null;
    public double? MaxValue { get; set; } = null;
    public double? MinValue { get; set; } = null;

    public ApplicationCommandOptionChoiceProperties[] Choices { get; set; } =
        Array.Empty<ApplicationCommandOptionChoiceProperties>();
    public ChannelType[] ChannelTypes { get; set; } = Array.Empty<ChannelType>();
    public ICommandOption[] Options { get; set; } = Array.Empty<ICommandOption>();

    public CommandOption(string name, string description, ApplicationCommandOptionType type)
    {
        Name = name;
        Description = description;
        Type = type;
    }
}