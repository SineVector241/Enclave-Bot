using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Preconditions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Actions
{
    [Group("action", "Action commands.")]
    [RequireContext(ContextType.Guild)]
    [IsStaff]
    public class Commands(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("list", "Lists all server actions")]
        public async Task ListActions()
        {
            var serverActions = (await database.Servers
                .Include(x => x.ServerActionsSettings)
                .ThenInclude(x => x.ActionGroups)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ServerActionsSettings.ActionGroups;

            var appList = Utils.CreateServerActionsList(serverActions.ToArray(), 0, Context.User);
            await RespondAsync(embed: appList.Item1, components: appList.Item2);
        }
    }
}