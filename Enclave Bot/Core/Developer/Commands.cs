using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;

namespace Enclave_Bot.Core.Developer
{
    [Group("dev", "Developer commands.")]
    public class Commands(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private readonly DatabaseContext Database = database;
    }
}
