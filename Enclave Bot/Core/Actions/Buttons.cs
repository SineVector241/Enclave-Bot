using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Preconditions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Actions
{
    [IsStaff]
    public class Buttons(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction($"{Constants.SERVER_ACTION_GROUP_LIST_NAVIGATE}:*,*")]
        public async Task ActionGroupListNavigate(string sAuthorId, string sPage)
        {
            var authorId = ulong.Parse(sAuthorId);
            var page = int.Parse(sPage);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this action group list!", ephemeral: true);
                return;
            }

            if (page < 0)
            {
                await RespondAsync("Page must be greater than or equal to zero", ephemeral: true);
                return;
            }

            var actionGroups = (await database.Servers
                .Include(x => x.ActionsSettings)
                .ThenInclude(x => x.ActionGroups)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ActionsSettings.ActionGroups;
            await DeferAsync();

            //Don't really care if it fails.
            var actionGroupsList = Utils.CreateActionGroupsList(actionGroups.ToArray(), page, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = actionGroupsList.Item1;
                x.Components = actionGroupsList.Item2;
            });
        }
        
        [ComponentInteraction($"{Constants.SERVER_ACTION_GROUP_LIST_CREATE}:*")]
        public async Task ActionGroupListCreate(string sAuthorId)
        {
            var authorId = ulong.Parse(sAuthorId);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this actions list!", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<CreateActionGroupModal>($"{Constants.SERVER_ACTION_GROUP_MODAL_CREATE}");
        }
    }
}