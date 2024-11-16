using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Actions
{
    [Group("actions", "Manages actions for the bot.")]
    [RequireContext(ContextType.Guild)]
    public class Commands(DatabaseContext database, Utils utils) : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private readonly DatabaseContext Database = database;
        private readonly Utils Utils = utils;

        [SlashCommand("create", "Creates a new action.")]
        public async Task Create(string name)
        {
            if(name.Length > Bot.NameLengthLimit)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"Name cannot be longer than {Bot.NameLengthLimit} characters!", Context.User));
                return;
            }

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);

            var action = new ServerAction() { ServerId = server.Id, Name = name };
            Database.ServerActions.Add(action);
            await Context.Interaction.DeferSafelyAsync();
            await Database.SaveChangesAsync();
            await Context.Interaction.RespondOrFollowupAsync($"Successfully created the action `{action.Name}` with the id `{action.Id}`.");
        }

        [SlashCommand("list", "lists all actions.")]
        public async Task List()
        {
            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
            var actions = Database.ServerActions.Where(x => x.ServerId == server.Id);

            var embed = Utils.CreateServerActionsListEmbed(Context.Guild, [..actions]);
            await Context.Interaction.RespondOrFollowupAsync(embed: embed.Build());
        }

        [SlashCommand("edit", "Shows the editor for an action.")]
        public async Task Edit(string id)
        {
            _ = Guid.TryParse(id, out var uuid);
            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
            var action = await Database.ServerActions.Where(x => x.ServerId == server.Id).FirstOrDefaultAsync(x => x.Id == uuid);

            if (action == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"An action with the id `{id}` does not exist!", Context.User));
                return;
            }

            var embed = Utils.CreateServerActionBehaviorsEditorEmbed(action, Context.User);
            var components = Utils.CreateServerActionBehaviorsEditorComponents(action, Context.User);
            await Context.Interaction.RespondOrFollowupAsync(embed: embed.Build(), components: components.Build());
        }
    }
}