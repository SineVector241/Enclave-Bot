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
        [ModalInteraction($"{Constants.SERVER_ACTION_MODAL_CREATE}")]
        public async Task CreateApplication(CreateActionModal modal)
        {
            if (modal.Name.Length > Constants.SERVER_ACTION_TITLE_CHARACTER_LIMIT)
            {
                await RespondAsync($"Name can only be {Constants.SERVER_ACTION_TITLE_CHARACTER_LIMIT} characters!", ephemeral: true);
                return;
            }

            var serverActionSettings = (await database.Servers
                .Include(x => x.ServerActionsSettings)
                .ThenInclude(x => x.ActionGroups)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ServerActionsSettings;
            var serverActions = serverActionSettings.ActionGroups;
            
            serverActions.Add(new ServerActionGroup { ServerActionsSettings = serverActionSettings, Name = modal.Name });
            await database.SaveChangesAsync();
            await RespondAsync($"Created action {modal.Name}.", ephemeral: true);
            
            //Don't really care if it fails.
            var applicationList = Utils.CreateServerActionsList(serverActions.ToArray(), 0, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationList.Item1;
                x.Components = applicationList.Item2;
            });
        }
    }
    
    public class CreateActionModal : IModal
    {
        public string Title => "Create Action";

        [InputLabel("Name")]
        [ModalTextInput("name", maxLength: Constants.APPLICATION_TITLE_CHARACTER_LIMIT)]
        public string Name { get; set; } = string.Empty;
    }
}