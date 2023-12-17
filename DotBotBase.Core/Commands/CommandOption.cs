using Discord;

namespace DotBotBase.Core.Commands;

public abstract class CommandOption
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract ApplicationCommandOptionType Type { get; }

    public virtual bool IsRequired { get; } = false;
    public virtual bool IsAutoComplete { get; } = false;
    public virtual int? MaxLength { get; } = null;
    public virtual int? MinLength { get; } = null;
    public virtual double? MaxValue { get; } = null;
    public virtual double? MinValue { get; } = null;

    public ApplicationCommandOptionChoiceProperties[] Choices { get; set; } =
        Array.Empty<ApplicationCommandOptionChoiceProperties>();
    public ChannelType[] ChannelTypes { get; set; } = Array.Empty<ChannelType>();

    public CommandOption[] Options { get; set; } = Array.Empty<CommandOption>();

    private Dictionary<string, CommandOption> _options = new Dictionary<string, CommandOption>();
    public CommandOption? GetOption(string command)
    {
        if (Options.Length != _options.Count)
        {
            _options = new Dictionary<string, CommandOption>();
            foreach (CommandOption option in Options)
                _options.Add(option.Name, option);
        }
        return _options.GetValueOrDefault(command);
    }
}