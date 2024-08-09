using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;

namespace Enclave_Bot.Core.Staff
{
    [Group("staff", "staff commands.")]
    [RequireContext(ContextType.Guild)]
    public class Commands(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private readonly DatabaseContext Database = database;
    }
}
