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
            if (name.Length > Constants.TitleLimit)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"Name cannot be longer than {Constants.TitleLimit} characters!", Context.User));
                return;
            }

            if ((await Database.GetServerApplications(Context.Guild.Id)).Length >= Constants.ApplicationsLimit)
            {
                await Context.Interaction.RespondOrFollowupAsync($"Cannot create application as application limit of {Constants.ApplicationsLimit} has been reached!");
                return;
            }

            var serverApplicationSettings = await Database.ServerApplicationSettings.FirstAsync(x => x.ServerId == Context.Guild.Id);
            var application = new Application() { ApplicationSettingsId = serverApplicationSettings.Id, Name = name };
            Database.ServerApplications.Add(application);
            await Context.Interaction.DeferSafelyAsync();
            await Database.SaveChangesAsync();
            await Context.Interaction.RespondOrFollowupAsync($"Successfully created the application `{application.Name}` with the id `{application.Id}`.");
        }

        [SlashCommand("delete", "Deletes an application.")]
        public async Task Delete(string applicationId)
        {
            if (!Guid.TryParse(applicationId, out var appId))
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"{applicationId} is an invalid application id!", Context.User));
                return;
            }

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"An application with the id `{applicationId}` does not exist!", Context.User));
                return;
            }

            await Context.Interaction.DeferSafelyAsync();
            await Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).ExecuteDeleteAsync();
            Database.ServerApplications.Remove(application);
            await Database.SaveChangesAsync();
            await Context.Interaction.RespondOrFollowupAsync($"Successfully deleted the application `{application.Name}` with the id `{application.Id}`.");
        }

        [SlashCommand("list", "Lists all applications")]
        public async Task List()
        {
            var applications = await Database.GetServerApplications(Context.Guild.Id);

            var embed = Utils.CreateApplicationListEmbed(Context.Guild, applications, Context.User);
            var components = Utils.CreateApplicationListComponents(applications, Context.User);
            await Context.Interaction.RespondOrFollowupAsync(embed: embed.Build(), components: components.Build());
        }

        [SlashCommand("edit", "Shows the editor for an application.")]
        public async Task Edit(string applicationId)
        {
            if (!Guid.TryParse(applicationId, out var appId))
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"{applicationId} is an invalid application id!", Context.User));
                return;
            }

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"An application with the id `{applicationId}` does not exist!", Context.User));
                return;
            }

            var embed = Utils.CreateApplicationEditorEmbed(application, Context.User);
            var components = Utils.CreateApplicationEditorComponents(application, Context.User);
            await Context.Interaction.RespondOrFollowupAsync(embed: embed.Build(), components: components.Build());
        }
    }
}