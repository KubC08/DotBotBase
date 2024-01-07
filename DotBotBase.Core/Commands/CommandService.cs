using Discord;
using Discord.WebSocket;
using DotBotBase.Core.Logging;

namespace DotBotBase.Core.Commands;

/// <summary>
/// The service responsible for handling all command executions and generating.
/// </summary>
public static class CommandService
{
    private static readonly Logger _log = new Logger("Command Service", DotBotInfo.Name);

    private static readonly Dictionary<string, Command> _commands = new Dictionary<string, Command>();
    /// <summary>
    /// The list of currently loaded and active commands.
    /// </summary>
    public static Command[] Commands => _commands.Values.ToArray();
    
    /// <summary>
    /// Invoke a slash command, usually attached to an event that automatically calls the function when a user calls a command.
    /// </summary>
    /// <param name="slashCommand">The Discord.NET slash command instance.</param>
    public static async Task RunCommand(SocketSlashCommand slashCommand)
    {
        if (!_commands.TryGetValue(slashCommand.Data.Name, out var command)) return;
        await _log.SafeInvoke($"Failed to run command {command.Name}", async Task () => await RunCommand(command, slashCommand, null, null));
    }

    private static async Task RunCommand(Command command, SocketSlashCommand slashCommand, ICommandOption? option, SocketSlashCommandDataOption? slashOption)
    {
        ICommandOption[] commandOptions = option?.Options ?? command.Options;
        var slashOptions = slashOption?.Options ?? slashCommand.Data.Options;
        if (slashOptions == null) return;

        Dictionary<string, object> args = new Dictionary<string, object>();
        if (commandOptions.Length > 0 && slashOptions.Count > 0)
        {
            foreach (var subSlashOption in slashOptions)
            {
                ICommandOption? subOption = GetOptionByName(commandOptions, subSlashOption.Name);
                if (subOption?.Type == null) continue;

                if (subOption.Type == ApplicationCommandOptionType.SubCommand)
                {
                    await RunCommand((Command)subOption, slashCommand, subOption, subSlashOption);
                    return;
                }
                if (subOption.Type == ApplicationCommandOptionType.SubCommandGroup)
                {
                    await RunCommand(command, slashCommand, subOption, subSlashOption);
                    return;
                }
                
                args.Add(subSlashOption.Name, subSlashOption.Value);
            }
        }
        
        _log.LogDebug($"Running command {command.Name}");
        await command.Run(slashCommand, args);
    }

    /// <summary>
    /// Loads a command of a specific type into the service so it can be built and ran.
    /// </summary>
    /// <typeparam name="T">The command type that must extend "Command" abstract class.</typeparam>
    /// <returns>The instance of the command of type "Command".</returns>
    public static Command? LoadCommand<T>() where T : Command, new()
    {
        _log.LogInfo($"Loading command of type {typeof(T).Name}...");

        T? command = null;
        _log.SafeInvoke($"Failed to load command {typeof(T).Name}", () => command = new T());
        if (command == null || command.Name == null || command.Description == null) return null;
        
        _commands.Add(command.Name, command);
        _log.LogInfo($"Successfully loaded command {command.Name}");
        return command;
    }

    private static ICommandOption? GetOptionByName(ICommandOption[] options, string name)
    {
        foreach (ICommandOption option in options)
            if (option.Name == name) return option;
        return null;
    }
    
    /// <summary>
    /// Build a specific "Command" instance to be usable with Discord.NET.
    /// </summary>
    /// <param name="command">The target command instance to build.</param>
    /// <returns>The built output which can be loaded into Discord.NET.</returns>
    public static SlashCommandProperties Build(Command command)
    {
        _log.LogDebug($"Building command {command.Name}");
        SlashCommandBuilder builder = new SlashCommandBuilder();
        builder.WithName(command.Name);
        builder.WithDescription(command.Description);
        builder.WithNsfw(command.IsNsfw);
        builder.AddOptions(BuildOptions(command.Options));
        return builder.Build();
    }
    
    private static SlashCommandOptionBuilder[] BuildOptions(ICommandOption[] options)
    {
        List<SlashCommandOptionBuilder> output = new List<SlashCommandOptionBuilder>();
        foreach (ICommandOption option in options)
        {
            if (option.Name == null || option.Description == null || option.Type == null) continue;
            _log.LogDebug($"Building command option {option.Name}");
            
            SlashCommandOptionBuilder builder = new SlashCommandOptionBuilder();
            builder.WithName(option.Name);
            builder.WithDescription(option.Description);
            builder.WithType((ApplicationCommandOptionType)option.Type);
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