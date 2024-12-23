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
        [SlashCommand("list", "Lists all server action groups.")]
        public async Task ListActionGroups()
        {
            var actionGroups = (await database.Servers
                .Include(x => x.ActionsSettings)
                .ThenInclude(x => x.ActionGroups)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ActionsSettings.ActionGroups;

            var actionGroupsList = Utils.CreateActionGroupsList(actionGroups.ToArray(), 0, Context.User);
            await RespondAsync(embed: actionGroupsList.Item1, components: actionGroupsList.Item2);
        }
    }
}