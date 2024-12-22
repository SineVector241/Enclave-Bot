using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Preconditions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Applications
{
    [Group("application", "Application commands.")]
    [RequireContext(ContextType.Guild)]
    [IsStaff]
    public class Commands(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("list", "Lists all applications.")]
        public async Task ListApplications()
        {
            var serverApplications = (await database.Servers
                .Include(x => x.ApplicationSettings)
                .ThenInclude(x => x.Applications)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ApplicationSettings.Applications;

            var appList = Utils.CreateApplicationList(serverApplications.ToArray(), 0, Context.User);
            await RespondAsync(embed: appList.Item1, components: appList.Item2);
        }
    }
}