using Discord.WebSocket;
using DotBotBase.Core.Commands;
using DotBotBase.Core.Logging;

namespace DotBotBase.Core.Modular;

/// <summary>
/// The overridable module class that indicates that the assembly is a module.
/// </summary>
public abstract class BotModule
{
    internal bool IsRunning = false;

    private readonly List<Command> _commands = new List<Command>();
    /// <summary>
    /// All the commands attached to the current module.
    /// </summary>
    public Command[] Commands => _commands.ToArray();
    
    /// <summary>
    /// The Name of the module.
    /// </summary>
    public abstract string Name { get; }
    /// <summary>
    /// The Version of the module.
    /// </summary>
    public abstract string Version { get; }
    /// <summary>
    /// The Author of the module.
    /// </summary>
    public abstract string Author { get; }
    /// <summary>
    /// If active, the asynchronous functions wait to be finished before continuing.
    /// </summary>
    public virtual bool ShouldWait => false;
    
    /// <summary>
    /// The basic Logger attached to the Module
    /// </summary>
    public Logger? Log { get; internal set; }
    
    /// <summary>
    /// The DotBot client instance that the module is attached to.
    /// </summary>
    public DotBot? Client { get; internal set; }

    /// <summary>
    /// A synchronous function called when the module is started.
    /// </summary>
    public virtual void Start() {}
    /// <summary>
    /// The asynchronous version of the Start function.
    /// If the "ShouldWait" is active then wait for function to finish then continue loading.
    /// </summary>
    /// <returns>The "void" Task.</returns>
    public virtual Task StartAsync() => Task.CompletedTask;
    /// <summary>
    /// A synchronous function called when the module is stopped.
    /// </summary>
    public virtual void Shutdown() {}
    /// <summary>
    /// The asynchronous version of the Shutdown function.
    /// If the "ShouldWait" is active then wait for function to finish then continue unloading.
    /// </summary>
    /// <returns></returns>
    public virtual Task ShutdownAsync() => Task.CompletedTask;

    /// <summary>
    /// Adds a global slash command of the given generic type.
    /// </summary>
    /// <typeparam name="T">The command of type "Command".</typeparam>
    /// <returns>The instance of the "Command" type specified in generic.</returns>
    public T? LoadGlobalCommand<T>() where T : Command, new()
    {
        if (!IsRunning) throw new Exception("Module is not running!");

        Command? command = CommandService.LoadGlobalCommand<T>();
        if (command == null) return null;
        
        _commands.Add(command);
        return (T)command;
    }

    /// <summary>
    /// Adds a guild slash command of the given generic type.
    /// </summary>
    /// <typeparam name="T">The command of type "Command".</typeparam>
    /// <param name="guildId">The id of the guild you would like to add the command to.</param>
    /// <returns>The instance of the "Command" type specified in generic.</returns>
    public T? LoadGuildCommand<T>(ulong guildId) where T : Command, new()
    {
        if (!IsRunning) throw new Exception("Module is not running!");

        Command? command = CommandService.LoadGuildCommand<T>(guildId);
        if (command == null) return null;
        
        _commands.Add(command);
        return (T)command;
    }
    
    /// <summary>
    /// Adds a guild slash command of the given generic type.
    /// </summary>
    /// <typeparam name="T">The command of type "Command".</typeparam>
    /// <param name="guild">The guild you would like to add the command to.</param>
    /// <returns>The instance of the "Command" type specified in generic.</returns>
    public T? LoadGuildCommand<T>(SocketGuild guild) where T : Command, new()
    {
        if (!IsRunning) throw new Exception("Module is not running!");

        Command? command = CommandService.LoadGuildCommand<T>(guild.Id);
        if (command == null) return null;
        
        _commands.Add(command);
        return (T)command;
    }
}