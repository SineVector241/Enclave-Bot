using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;

namespace Enclave_Bot.Core.Actions
{
    [Group("actions", "Manages actions for the bot.")]
    public class Commands(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private readonly DatabaseContext Database = database;

        [SlashCommand("create", "creates a new action.")]
        public async Task Create(string name)
        {
            if(name.Length > Bot.NameLengthLimit)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"Name cannot be longer than {Bot.NameLengthLimit} characters!", Context.User), ephemeral: true);
                return;
            }

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);
            var actions = Database.ServerActions.Where(x => x.ServerId == server.Id);

            var action = new ServerAction() { Name = name, ServerId = server.Id, Server = server };
            Database.ServerActions.Add(action);
            await Context.Interaction.DeferSafelyAsync(ephemeral: true);
            await Database.SaveChangesAsync();
            await Context.Interaction.RespondOrFollowupAsync($"Successfully created the action `{action.Name}` with the id {action.Id}.", ephemeral: true);
        }

        [SlashCommand("list", "lists all actions.")]
        public async Task List()
        {
            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);
            var actions = Database.ServerActions.Where(x => x.ServerId == server.Id);

            var embed = Utils.CreateServerActionsListEmbed(Context.Guild, [.. actions]);
            await Context.Interaction.RespondOrFollowupAsync(embed: embed.Build(), ephemeral: true);
        }

        [SlashCommand("edit", "Shows the editor for an action.")]
        public async Task EditApplication(string id)
        {
            _ = Guid.TryParse(id, out Guid uuid);
            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);
            var action = Database.ServerActions.Where(x => x.ServerId == server.Id).FirstOrDefault(x => x.Id == uuid);

            if (action == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"An action with the id `{id}` does not exist!", Context.User), ephemeral: true);
                return;
            }

            var embed = Utils.CreateServerBehaviorsEditorEmbed(action, Context.User);
            var components = Utils.CreateServerBehaviorsEditorComponents(action, Context.User);
            await Context.Interaction.RespondOrFollowupAsync(embed: embed.Build(), components: components.Build());
        }
    }
}