using Discord;
using Discord.WebSocket;

namespace DotBotBase.Core.Commands;

public abstract class Command : CommandOptionImpl
{
    // Type is ignored on initial commands, so we can do this
    public override ApplicationCommandOptionType? Type => ApplicationCommandOptionType.SubCommand;

    public virtual bool IsNsfw { get; } = false;

    public virtual Task Run(SocketSlashCommand command, Dictionary<string, object> argument) => Task.CompletedTask;
}