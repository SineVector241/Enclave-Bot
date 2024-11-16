using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;

namespace Enclave_Bot.Core.Applications
{
    [Group("application", "Manages applications for the bot.")]
    [RequireContext(ContextType.Guild)]
    public class Commands(DatabaseContext database, Utils utils) : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private readonly DatabaseContext Database = database;
        private readonly Utils Utils = utils;

        [SlashCommand("list", "Lists all applications")]
        public async Task List()
        {
            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
            var applications = Database.ServerApplications.Where(x => x.ApplicationSettingsId == server.ApplicationSettings.Id).ToArray();

            var embed = Utils.CreateApplicationListEmbed(Context.Guild, applications);
            await Context.Interaction.RespondOrFollowupAsync(embed: embed.Build());
        }
    }
}
