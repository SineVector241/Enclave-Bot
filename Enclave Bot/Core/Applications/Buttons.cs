using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;

namespace Enclave_Bot.Core.Applications
{
    public class Buttons(DatabaseContext database, Utils utils) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        private readonly DatabaseContext Database = database;
        private readonly Utils Utils = utils;
    }
}
