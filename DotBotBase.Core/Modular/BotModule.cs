using DotBotBase.Core.Commands;
using DotBotBase.Core.Logging;

namespace DotBotBase.Core.Modular;

public abstract class BotModule
{
    internal bool IsRunning = false;

    private List<Command> _commands = new List<Command>();
    public Command[] Commands => _commands.ToArray();
    
    public abstract string Name { get; }
    public abstract string Version { get; }
    public abstract string Author { get; }
    
    public Logger? Log { get; internal set; }

    public virtual void Start() {}
    public virtual void Shutdown() {}

    public T? AddCommand<T>() where T : Command, new()
    {
        if (!IsRunning) return null;

        Command? command = CommandService.LoadCommand<T>();
        if (command == null) return null;
        
        _commands.Add(command);
        return (T)command;
    }
}