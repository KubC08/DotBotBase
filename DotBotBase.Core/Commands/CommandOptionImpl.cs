using Discord;

namespace DotBotBase.Core.Commands;

public abstract class CommandOptionImpl : ICommandOption
{
    public abstract string? Name { get; }
    public abstract string? Description { get; }
    public abstract ApplicationCommandOptionType? Type { get; }

    public virtual bool IsRequired { get; } = false;
    public virtual bool IsAutoComplete { get; } = false;
    public virtual int? MaxLength { get; } = null;
    public virtual int? MinLength { get; } = null;
    public virtual double? MaxValue { get; } = null;
    public virtual double? MinValue { get; } = null;

    public virtual ApplicationCommandOptionChoiceProperties[] Choices { get; } =
        Array.Empty<ApplicationCommandOptionChoiceProperties>();
    public virtual ChannelType[] ChannelTypes { get; } = Array.Empty<ChannelType>();

    public virtual ICommandOption[] Options { get; } = Array.Empty<ICommandOption>();
}