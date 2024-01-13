using Discord.Rest;
using Discord.WebSocket;

namespace DotBotBase.Core.Commands;

public static class CommandExtensions
{
    /// <summary>
    /// Gets all the available slash commands that a guild has assigned by the bot.
    /// </summary>
    /// <param name="guild">The target guild to get the commands from.</param>
    /// <param name="client">The client responsible for the guild.</param>
    /// <returns>The list of slash commands in the selected guild.</returns>
    public static async Task<IReadOnlyCollection<RestGuildCommand>?> GetAllCommands(this SocketGuild guild, DotBot client)
    {
        if (client.Client == null) return null;
        return await client.Client.Rest.GetGuildApplicationCommands(guild.Id);
    }
}