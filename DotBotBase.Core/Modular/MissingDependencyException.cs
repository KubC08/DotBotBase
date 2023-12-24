namespace DotBotBase.Core.Modular;

public class MissingDependencyException : Exception
{
    public MissingDependencyException(string dependency) : base("Missing dependency " + dependency) {}
}