using Discord;

namespace DotBotBase.Core.Commands;

/// <summary>
/// A base command option used for creating generic non-command options.
/// </summary>
public class CommandOption : ICommandOption
{
    /// <summary>
    /// The name of the option that is the "key" of the Discord option.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// The description of the slash command that is shown on Discord.
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// The input type of the command option.
    /// </summary>
    public ApplicationCommandOptionType? Type { get; set; }

    /// <summary>
    /// Is the user required to set this specific option.
    /// Default: false
    /// </summary>
    public bool IsRequired { get; set; } = false;
    /// <summary>
    /// Should the option allow auto-completing values.
    /// Default: false
    /// </summary>
    public bool IsAutoComplete { get; set; } = false;
    /// <summary>
    /// The maximum length of the option's value (only for strings). If null it is the Discord default.
    /// Default: null
    /// </summary>
    public int? MaxLength { get; set; } = null;
    /// <summary>
    /// The minimum length of the option's value (only for strings). If null it is the Discord default.
    /// Default: null
    /// </summary>
    public int? MinLength { get; set; } = null;
    /// <summary>
    /// The highest possible value that could be given if value type is a number. If null it is the Discord default.
    /// Default: null
    /// </summary>
    public double? MaxValue { get; set; } = null;
    /// <summary>
    /// The lowest possible value that could be given if value type is a number. If null it is the Discord default.
    /// Default: null
    /// </summary>
    public double? MinValue { get; set; } = null;

    /// <summary>
    /// Restrict the values to a select given choices for the option. Use empty array to disable it.
    /// Default: empty array
    /// </summary>
    public ApplicationCommandOptionChoiceProperties[] Choices { get; set; } =
        Array.Empty<ApplicationCommandOptionChoiceProperties>();
    /// <summary>
    /// Restrict which channels can be selected as the value if the Type is selected as "Channel".
    /// </summary>
    public ChannelType[] ChannelTypes { get; set; } = Array.Empty<ChannelType>();
    /// <summary>
    /// Sub-options for the current option.
    /// </summary>
    public ICommandOption[] Options { get; set; } = Array.Empty<ICommandOption>();

    /// <summary>
    /// Create a basic/generic option that can be used for almost everything but commands.
    /// </summary>
    /// <param name="name">The name of the option that is the "key" of the Discord option.</param>
    /// <param name="description">The description of the slash command that is shown on Discord.</param>
    /// <param name="type">The input type of the command option.</param>
    public CommandOption(string name, string description, ApplicationCommandOptionType type)
    {
        Name = name;
        Description = description;
        Type = type;
    }
}