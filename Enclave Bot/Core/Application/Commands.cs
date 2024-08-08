using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;

namespace Enclave_Bot.Core.Application
{
    [Group("application", "application commands.")]
    [RequireContext(ContextType.Guild)]
    public class Commands(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private readonly DatabaseContext Database = database;

        [SlashCommand("list", "Lists all created applications.")]
        public async Task ListApplications()
        {
            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);
            var applications = Database.ServerApplications.Where(x => x.ServerId == server.Id);

            var embed = Utils.CreateApplicationListEmbed(Context.Guild, applications.ToArray());
            await Context.Interaction.RespondOrFollowupAsync(embed: embed.Build(), ephemeral: true);
        }

        [SlashCommand("create", "Creates an application.")]
        public async Task CreateApplication(string name)
        {
            if (name.Length > Bot.NameLengthLimit)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"Name cannot be longer than {Bot.NameLengthLimit} characters!", Context.User), ephemeral: true);
                return;
            }

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);

            var application = new ServerApplication() { ServerId = server.Id, Server = server, Name = name, Questions = new List<string>() };
            Database.ServerApplications.Add(application);
            await Context.Interaction.DeferSafelyAsync(ephemeral: true);
            await Database.SaveChangesAsync();
            await Context.Interaction.RespondOrFollowupAsync($"Successfully created the application `{name}` with id `{application.Id}`.", ephemeral: true);
        }

        [SlashCommand("edit", "Shows the editor for an application.")]
        public async Task EditApplication(string id)
        {
            Guid.TryParse(id, out var uuid);
            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);
            var application = Database.ServerApplications.Where(x => x.ServerId == server.Id).FirstOrDefault(x => x.Id == uuid);

            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"An application with the id `{id}` does not exist!", Context.User), ephemeral: true);
                return;
            }

            var embed = Utils.CreateApplicationEditorEmbed(application, Context.User);
            var components = Utils.CreateApplicationEditorComponents(application, Context.User);
            await Context.Interaction.RespondOrFollowupAsync(embed: embed.Build(), components: components.Build());
        }

        [SlashCommand("delete", "Deletes an application.")]
        public async Task DeleteApplication(string id)
        {
            Guid.TryParse(id, out var uuid);
            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);
            var application = Database.ServerApplications.Where(x => x.ServerId == server.Id).FirstOrDefault(x => x.Id == uuid);

            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"An application with the id `{id}` does not exist!", Context.User), ephemeral: true);
                return;
            }

            Database.ServerApplications.Remove(application);
            await Context.Interaction.DeferSafelyAsync(ephemeral: true);
            await Database.SaveChangesAsync();
            await Context.Interaction.RespondOrFollowupAsync($"Successfully deleted the application `{application.Name}` with the id {id}.", ephemeral: true);
        }
    }
}
