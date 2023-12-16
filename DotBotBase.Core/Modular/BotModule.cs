using DotBotBase.Core.Logging;

namespace DotBotBase.Core.Modular;

public abstract class BotModule
{
    internal bool IsRunning = false;
    
    public abstract string Name { get; }
    public abstract string Version { get; }
    public abstract string Author { get; }
    
    public Logger? Log { get; internal set; }

    public virtual void Start() {}

    public virtual void Shutdown() {}
}