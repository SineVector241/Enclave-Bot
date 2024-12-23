using Discord;
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
        public async Task CreateServerActionGroup(CreateActionGroupModal modal)
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
            var actionGroups = serverActionSettings.ActionGroups;
            
            actionGroups.Add(new ActionGroup { ActionsSettings = serverActionSettings, Name = modal.Name });
            await database.SaveChangesAsync();
            await RespondAsync($"Created action group {modal.Name}.", ephemeral: true);
            
            //Don't really care if it fails.
            var actionGroupsList = Utils.CreateActionGroupsList(actionGroups.ToArray(), 0, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = actionGroupsList.Item1;
                x.Components = actionGroupsList.Item2;
            });
        }
    }
    
    public class CreateActionGroupModal : IModal
    {
        public string Title => "Create Action Group";

        [InputLabel("Name")]
        [ModalTextInput("name", maxLength: Constants.SERVER_ACTION_GROUP_TITLE_CHARACTER_LIMIT)]
        public string Name { get; set; } = string.Empty;
    }
}