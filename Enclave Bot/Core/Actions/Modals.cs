using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Preconditions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Actions
{
    [IsStaff]
    public class Modals(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketModal>>
    {
        [ModalInteraction($"{Constants.SERVER_ACTION_GROUP_MODAL_CREATE}")]
        public async Task CreateServerActionGroup(CreateServerActionGroupModal modal)
        {
            if (modal.Name.Length > Constants.SERVER_ACTION_GROUP_TITLE_CHARACTER_LIMIT)
            {
                await RespondAsync($"Name can only be {Constants.SERVER_ACTION_GROUP_TITLE_CHARACTER_LIMIT} characters!", ephemeral: true);
                return;
            }

            var serverActionSettings = (await database.Servers
                .Include(x => x.ActionsSettings)
                .ThenInclude(x => x.ActionGroups)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ActionsSettings;
            var serverActionGroups = serverActionSettings.ActionGroups;
            
            serverActionGroups.Add(new ActionGroup { ActionsSettings = serverActionSettings, Name = modal.Name });
            await database.SaveChangesAsync();
            await RespondAsync($"Created action group {modal.Name}.", ephemeral: true);
            
            //Don't really care if it fails.
            var actionGroupsList = Utils.CreateServerActionGroupsList(serverActionGroups.ToArray(), 0, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = actionGroupsList.Item1;
                x.Components = actionGroupsList.Item2;
            });
        }
        
        [ModalInteraction($"{Constants.SERVER_ACTION_MODAL_CREATE}:*")]
        public async Task CreateServerAction(string sActionGroupId, CreateServerActionModal modal)
        {
            var actionGroupId = long.Parse(sActionGroupId);
            
            if (modal.Name.Length > Constants.SERVER_ACTION_TITLE_CHARACTER_LIMIT)
            {
                await RespondAsync($"Name can only be {Constants.SERVER_ACTION_TITLE_CHARACTER_LIMIT} characters!", ephemeral: true);
                return;
            }

            var serverActionGroup = (await database.Servers
                .Include(x => x.ActionsSettings)
                .ThenInclude(x => x.ActionGroups)
                .ThenInclude(x => x.Actions)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ActionsSettings.ActionGroups.FirstOrDefault(x => x.Id == actionGroupId);

            if (serverActionGroup == null)
            {
                await RespondAsync($"Action group with id {actionGroupId} was not found!", ephemeral: true);
                return;
            }
            
            serverActionGroup.Actions.Add(new ServerAction { ActionGroup = serverActionGroup, Name = modal.Name });
            await database.SaveChangesAsync();
            await RespondAsync($"Created action {modal.Name}.", ephemeral: true);
            
            //Don't really care if it fails.
            var actionsList = Utils.CreateServerActionsList(serverActionGroup, 0, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = actionsList.Item1;
                x.Components = actionsList.Item2;
            });
        }
    }
    
    public class CreateServerActionGroupModal : IModal
    {
        public string Title => "Create Action Group";

        [InputLabel("Name")]
        [ModalTextInput("name", maxLength: Constants.SERVER_ACTION_GROUP_TITLE_CHARACTER_LIMIT)]
        public string Name { get; set; } = string.Empty;
    }

    public class CreateServerActionModal : IModal
    {
        public string Title => "Create Action";

        [InputLabel("Name")]
        [ModalTextInput("name", maxLength: Constants.SERVER_ACTION_GROUP_TITLE_CHARACTER_LIMIT)]
        public string Name { get; set; } = string.Empty;
    }
}