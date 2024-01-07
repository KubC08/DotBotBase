using Discord;
using Discord.WebSocket;

namespace DotBotBase.Core.Commands;

/// <summary>
/// Inherit to indicate either a top-level command or a sub-command.
/// </summary>
public abstract class Command : ICommandOption
{
    /// <summary>
    /// The name of the option that is the "key" of the Discord option.
    /// </summary>
    public abstract string? Name { get; }
    /// <summary>
    /// The description of the slash command that is shown on Discord.
    /// </summary>
    public abstract string? Description { get; }
    
    // Type is ignored on initial commands, so we can do this
    /// <summary>
    /// The input type of the command option.
    /// </summary>
    public ApplicationCommandOptionType? Type => ApplicationCommandOptionType.SubCommand;

    /// <summary>
    /// Is the command considered NSFW either in it's input or output.
    /// Default: false
    /// </summary>
    public virtual bool IsNsfw { get; } = false;
    
    /// <summary>
    /// Is the user required to set this specific option.
    /// Default: false
    /// </summary>
    public virtual bool IsRequired { get; } = false;
    /// <summary>
    /// Should the option allow auto-completing values.
    /// Default: false
    /// </summary>
    public virtual bool IsAutoComplete { get; } = false;
    /// <summary>
    /// The maximum length of the option's value (only for strings). If null it is the Discord default.
    /// Default: null
    /// </summary>
    public virtual int? MaxLength { get; } = null;
    /// <summary>
    /// The minimum length of the option's value (only for strings). If null it is the Discord default.
    /// Default: null
    /// </summary>
    public virtual int? MinLength { get; } = null;
    /// <summary>
    /// The highest possible value that could be given if value type is a number. If null it is the Discord default.
    /// Default: null
    /// </summary>
    public virtual double? MaxValue { get; } = null;
    /// <summary>
    /// The lowest possible value that could be given if value type is a number. If null it is the Discord default.
    /// Default: null
    /// </summary>
    public virtual double? MinValue { get; } = null;

    /// <summary>
    /// Restrict the values to a select given choices for the option. Use empty array to disable it.
    /// Default: empty array
    /// </summary>
    public virtual ApplicationCommandOptionChoiceProperties[] Choices { get; } =
        Array.Empty<ApplicationCommandOptionChoiceProperties>();
    /// <summary>
    /// Restrict which channels can be selected as the value if the Type is selected as "Channel".
    /// </summary>
    public virtual ChannelType[] ChannelTypes { get; } = Array.Empty<ChannelType>();

    /// <summary>
    /// Sub-options for the current option.
    /// </summary>
    public virtual ICommandOption[] Options { get; } = Array.Empty<ICommandOption>();

    /// <summary>
    /// Execute when the command is called/executed by a user.
    /// Do note that this is only executed at the highest level such as a sub-command if present.
    /// </summary>
    /// <param name="command">The Discord.NET slash command that was executed.</param>
    /// <param name="args">The arguments/options passed to the command in the format of (key, value).</param>
    /// <returns>The Task for asynchronous execution.</returns>
    public virtual Task Run(SocketSlashCommand command, Dictionary<string, object> args) => Task.CompletedTask;
}