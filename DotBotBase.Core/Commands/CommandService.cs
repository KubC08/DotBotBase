using Discord;
using DotBotBase.Core.Logging;
using DotBotBase.Core.Modular;

namespace DotBotBase.Core.Commands;

public static class CommandService
{
    private static readonly Logger _log = new Logger("Command Service", DotBotInfo.Name);

    private static readonly List<Command> _commands = new List<Command>();
    public static Command[] Commands => _commands.ToArray();

    public static Command? LoadCommand<T>() where T : Command, new()
    {
        _log.LogInfo($"Loading command of type {typeof(T).Name}...");

        T? command = null;
        _log.SafeInvoke($"Failed to load command {typeof(T).Name}", () => command = new T());
        if (command == null) return null;
        
        _commands.Add(command);
        _log.LogInfo($"Successfully loaded command {command.Name}");
        return command;
    }
    
    public static SlashCommandProperties Build(Command command)
    {
        SlashCommandBuilder builder = new SlashCommandBuilder();
        builder.WithName(command.Name);
        builder.WithDescription(command.Description);
        builder.WithNsfw(command.IsNsfw);
        builder.AddOptions(BuildOptions(command.Options));
        return builder.Build();
    }
    
    private static SlashCommandOptionBuilder[] BuildOptions(CommandOption[] options)
    {
        List<SlashCommandOptionBuilder> output = new List<SlashCommandOptionBuilder>();
        foreach (CommandOption option in options)
        {
            SlashCommandOptionBuilder builder = new SlashCommandOptionBuilder();
            builder.WithName(option.Name);
            builder.WithDescription(option.Description);
            builder.WithType(option.Type);
            builder.WithRequired(option.IsRequired);
            builder.WithAutocomplete(option.IsAutoComplete);
            builder.MaxLength = option.MaxLength;
            builder.MinLength = option.MinLength;
            builder.MaxValue = option.MaxValue;
            builder.MinValue = option.MinValue;

            foreach (var choice in option.Choices)
            {
                Type choiceType = choice.GetType();
                if (choiceType == typeof(int))
                    builder.AddChoice(choice.Name, (int)choice.Value, choice.NameLocalizations);
                else if (choiceType == typeof(string))
                    builder.AddChoice(choice.Name, (string)choice.Value, choice.NameLocalizations);
                else if (choiceType == typeof(double))
                    builder.AddChoice(choice.Name, (double)choice.Value, choice.NameLocalizations);
                else if (choiceType == typeof(float))
                    builder.AddChoice(choice.Name, (float)choice.Value, choice.NameLocalizations);
                else if (choiceType == typeof(long))
                    builder.AddChoice(choice.Name, (long)choice.Value, choice.NameLocalizations);
            }
            foreach (ChannelType channelType in option.ChannelTypes)
                builder.AddChannelType(channelType);

            builder.AddOptions(BuildOptions(option.Options));
            output.Add(builder);
        }
        return output.ToArray();
    }
}