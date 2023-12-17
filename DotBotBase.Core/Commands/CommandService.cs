using Discord;
using Discord.WebSocket;
using DotBotBase.Core.Logging;

namespace DotBotBase.Core.Commands;

public static class CommandService
{
    private static readonly Logger _log = new Logger("Command Service", DotBotInfo.Name);

    private static readonly Dictionary<string, Command> _commands = new Dictionary<string, Command>();
    public static Command[] Commands => _commands.Values.ToArray();
    
    public static async Task RunCommand(SocketSlashCommand slashCommand)
    {
        if (!_commands.TryGetValue(slashCommand.Data.Name, out var command)) return;
        if (command.Options.Length > 0 && slashCommand.Data.Options.Count > 0)
        {
            SocketSlashCommandDataOption slashOption = slashCommand.Data.Options.First();
            CommandOptionExtendable? option = command.GetOption(slashCommand.Data.Name);
            if (option == null) return;
            
            await RunOption(command, slashCommand, option, slashOption);
            return;
        }

        await command.Run(slashCommand, null);
    }

    private static async Task RunOption(Command command, SocketSlashCommand slashCommand, CommandOptionExtendable optionExtendable, SocketSlashCommandDataOption slashOption)
    {
        if (optionExtendable.Options.Length > 0 && slashOption.Options.Count > 0)
        {
            SocketSlashCommandDataOption subSlashOption = slashOption.Options.First();
            CommandOptionExtendable? subOption = optionExtendable.GetOption(subSlashOption.Name);
            if (subOption == null) return;

            if (subOption is Command)
            {
                await RunOption((Command)subOption, slashCommand, subOption, subSlashOption);
                return;
            }

            await RunOption(command, slashCommand, subOption, subSlashOption);
            return;
        }
        
        await command.Run(slashCommand, slashOption.Value);
    }

    public static Command? LoadCommand<T>() where T : Command, new()
    {
        _log.LogInfo($"Loading command of type {typeof(T).Name}...");

        T? command = null;
        _log.SafeInvoke($"Failed to load command {typeof(T).Name}", () => command = new T());
        if (command == null) return null;
        
        _commands.Add(command.Name, command);
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
    
    private static SlashCommandOptionBuilder[] BuildOptions(CommandOptionExtendable[] options)
    {
        List<SlashCommandOptionBuilder> output = new List<SlashCommandOptionBuilder>();
        foreach (CommandOptionExtendable option in options)
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