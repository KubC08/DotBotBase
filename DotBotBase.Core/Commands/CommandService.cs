using System.Collections.ObjectModel;
using Discord;
using Discord.WebSocket;
using DotBotBase.Core.Logging;

namespace DotBotBase.Core.Commands;

/// <summary>
/// The service responsible for handling all command executions and generating.
/// </summary>
public static class CommandService
{
    private static readonly Logger _log = new Logger("Command Service", DotBot.Name);

    private static readonly Dictionary<ulong, Dictionary<string, Command>> _guildCommands =
        new Dictionary<ulong, Dictionary<string, Command>>();
    private static readonly Dictionary<string, Command> _globalCommands = new Dictionary<string, Command>();

    /// <summary>
    /// Runs when a global command is loaded into the service.
    /// </summary>
    public static event Action<Command>? OnGlobalCommandLoad;
    /// <summary>
    /// Runs when a guild command is loaded into the service.
    /// </summary>
    public static event Action<Command, ulong>? OnGuildCommandLoad;
    /// <summary>
    /// Runs when a command is executed.
    /// </summary>
    public static event Func<DotBot, SocketSlashCommand, Dictionary<string, object>, Task>? OnCommandRun;
    
    /// <summary>
    /// The list of currently loaded and active global commands.
    /// </summary>
    public static IReadOnlyCollection<Command> GlobalCommands => _globalCommands.Values.ToArray();

    /// <summary>
    /// The list of currently loaded and active guild commands.
    /// </summary>
    public static IReadOnlyDictionary<ulong, IReadOnlyCollection<Command>> GuildCommands
    {
        get
        {
            Dictionary<ulong, IReadOnlyCollection<Command>> result =
                new Dictionary<ulong, IReadOnlyCollection<Command>>();
            foreach (var guildCommands in _guildCommands)
                result.Add(guildCommands.Key, guildCommands.Value.Values.ToArray());
            return new ReadOnlyDictionary<ulong, IReadOnlyCollection<Command>>(result);
        }
    }

    /// <summary>
    /// Invoke a slash command, usually attached to an event that automatically calls the function when a user calls a command.
    /// </summary>
    /// <param name="client">The DotBot client that was used to execute the command.</param>
    /// <param name="slashCommand">The Discord.NET slash command instance.</param>
    public static async Task RunCommand(DotBot client, SocketSlashCommand slashCommand)
    {
        Command? command = null;
        if (slashCommand.GuildId != null)
            if (_guildCommands.TryGetValue((ulong)slashCommand.GuildId, out var commands))
                command = commands.GetValueOrDefault(slashCommand.Data.Name);
        if (command == null)
            if (!_globalCommands.TryGetValue(slashCommand.Data.Name, out command))
                command = null;
        if (command == null) return;
        
        await _log.SafeInvoke($"Failed to run command {command.Name}", async Task () => await RunCommand(client, command, slashCommand, null, null));
    }

    private static async Task RunCommand(DotBot client, Command command, SocketSlashCommand slashCommand, ICommandOption? option, SocketSlashCommandDataOption? slashOption)
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
                    await RunCommand(client, (Command)subOption, slashCommand, subOption, subSlashOption);
                    return;
                }
                if (subOption.Type == ApplicationCommandOptionType.SubCommandGroup)
                {
                    await RunCommand(client, command, slashCommand, subOption, subSlashOption);
                    return;
                }
                
                args.Add(subSlashOption.Name, subSlashOption.Value);
            }
        }
        
        _log.LogDebug($"Running command {command.Name}");
        await command.Run(client, slashCommand, args);
        await _log.SafeInvoke($"Failed to execute OnCommand event", () => OnCommandRun?.Invoke(client, slashCommand, args) ?? Task.CompletedTask);
    }

    private static Command? LoadCommand<T>() where T : Command, new()
    {
        T? command = null;
        _log.SafeInvoke($"Failed to load command {typeof(T).Name}", () => command = new T());
        
        if (command?.Name == null || command.Description == null) return null;
        return command;
    }

    /// <summary>
    /// Loads a global command of a specific type into the service so it can be built and ran.
    /// </summary>
    /// <typeparam name="T">The command type that must extend "Command" abstract class.</typeparam>
    /// <returns>The instance of the command of type "Command".</returns>
    public static Command? LoadGlobalCommand<T>() where T : Command, new()
    {
        _log.LogInfo($"Loading global command of type {typeof(T).Name}...");

        Command? command = LoadCommand<T>();
        if (command?.Name == null) return null;
        
        _globalCommands.Add(command.Name, command);
        _log.SafeInvoke($"Failed to execute global command load event for {command.Name}", () => OnGlobalCommandLoad?.Invoke(command));
        _log.LogInfo($"Successfully loaded global command {command.Name}");
        return command;
    }

    /// <summary>
    /// Loads a global command of a specific type into the service so it can be built and ran.
    /// </summary>
    /// <param name="guildId">The guild id to load the command into.</param>
    /// <typeparam name="T">The command type that must extend "Command" abstract class.</typeparam>
    /// <returns>The instance of the command of type "Command".</returns>
    public static Command? LoadGuildCommand<T>(ulong guildId) where T : Command, new()
    {
        _log.LogInfo($"Loading guild command of type {typeof(T).Name}...");

        Command? command = LoadCommand<T>();
        if (command?.Name == null) return null;

        if (!_guildCommands.TryGetValue(guildId, out var guildCommands))
        {
            guildCommands = new Dictionary<string, Command>();
            _guildCommands.Add(guildId, guildCommands);
        }
        guildCommands.Add(command.Name, command);

        _log.SafeInvoke($"Failed to execute guild command load event for {command.Name}", () => OnGuildCommandLoad?.Invoke(command, guildId));
        _log.LogInfo($"Successfully loaded guild command {command.Name}");
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