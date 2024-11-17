using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Applications
{
    [Group("application", "Manages applications for the bot.")]
    [RequireContext(ContextType.Guild)]
    public class Commands(DatabaseContext database, Utils utils) : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private readonly DatabaseContext Database = database;
        private readonly Utils Utils = utils;

        [SlashCommand("create", "Creates a new application.")]
        public async Task Create(string name)
        {
            if (name.Length > Bot.TitleLengthLimit)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"Name cannot be longer than {Bot.TitleLengthLimit} characters!", Context.User));
                return;
            }

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
            var serverApplicationSettings = await Database.ServerApplicationSettings.FirstAsync(x => x.ServerId == server.Id);

            var application = new Application() { ApplicationSettingsId = serverApplicationSettings.Id, Name = name };
            Database.ServerApplications.Add(application);
            await Context.Interaction.DeferSafelyAsync();
            await Database.SaveChangesAsync();
            await Context.Interaction.RespondOrFollowupAsync($"Successfully created the application `{application.Name}` with the id `{application.Id}`.");
        }

        [SlashCommand("list", "Lists all applications")]
        public async Task List()
        {
            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
            var serverApplicationSettings = await Database.ServerApplicationSettings.FirstAsync(x => x.ServerId == server.Id);
            var applications = Database.ServerApplications.Where(x => x.ApplicationSettingsId == serverApplicationSettings.Id).ToArray();

            var embed = Utils.CreateApplicationListEmbed(Context.Guild, applications, Context.User);
            var components = Utils.CreateApplicationListComponents(applications, Context.User);
            await Context.Interaction.RespondOrFollowupAsync(embed: embed.Build());
        }

        [SlashCommand("edit", "Shows the editor for an application.")]
        public async Task Edit(string id)
        {
            await Context.Interaction.DeferSafelyAsync();
            _ = Guid.TryParse(id, out var uuid);
            await Context.Interaction.DeferSafelyAsync();
            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
            var serverApplicationSettings = await Database.ServerApplicationSettings.FirstAsync(x => x.ServerId == server.Id);
            var application = await Database.ServerApplications.Where(x => x.ApplicationSettingsId == serverApplicationSettings.Id).FirstOrDefaultAsync(x => x.Id == uuid);

            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"An application with the id `{id}` does not exist!", Context.User));
                return;
            }

            var embed = Utils.CreateApplicationEditorEmbed(application, Context.User);
            var components = Utils.CreateApplicationEditorComponents(application, Context.User);
            await Context.Interaction.RespondOrFollowupAsync(embed: embed.Build(), components: components.Build());
        }
    }
}
