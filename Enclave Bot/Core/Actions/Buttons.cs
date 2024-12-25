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
        public async Task ServerActionGroupListNavigate(string sAuthorId, string sPage)
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

            var serverActionGroups = (await database.Servers
                .Include(x => x.ActionsSettings)
                .ThenInclude(x => x.ActionGroups)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ActionsSettings.ActionGroups;
            await DeferAsync();

            //Don't really care if it fails.
            var serverActionGroupsList = Utils.CreateServerActionGroupsList(serverActionGroups.ToArray(), page, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = serverActionGroupsList.Item1;
                x.Components = serverActionGroupsList.Item2;
            });
        }
        
        [ComponentInteraction($"{Constants.SERVER_ACTION_GROUP_LIST_CREATE}:*")]
        public async Task ServerActionGroupListCreate(string sAuthorId)
        {
            var authorId = ulong.Parse(sAuthorId);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this actions list!", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<CreateServerActionGroupModal>($"{Constants.SERVER_ACTION_GROUP_MODAL_CREATE}");
        }
        
        [ComponentInteraction($"{Constants.SERVER_ACTION_LIST_NAVIGATE}:*,*,*")]
        public async Task ServerActionListNavigate(string sAuthorId, string sActionGroupId, string sPage)
        {
            var authorId = ulong.Parse(sAuthorId);
            var actionGroupId = long.Parse(sActionGroupId);
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
            await DeferAsync();

            //Don't really care if it fails.
            var actionsList = Utils.CreateServerActionsList(serverActionGroup, page, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = actionsList.Item1;
                x.Components = actionsList.Item2;
            });
        }
        
        [ComponentInteraction($"{Constants.SERVER_ACTION_LIST_CREATE}:*,*")]
        public async Task ServerActionListCreate(string sAuthorId, string sActionGroupId)
        {
            var authorId = ulong.Parse(sAuthorId);
            var actionGroupId = long.Parse(sActionGroupId);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this actions list!", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<CreateServerActionModal>($"{Constants.SERVER_ACTION_MODAL_CREATE}:{actionGroupId}");
        }

        [ComponentInteraction($"{Constants.SERVER_ACTION_EDIT_SETTINGS}:*,*,*")]
        public async Task ServerActionEditSettings(string sAuthorId, string sActionGroupId, string sActionId)
        {
            
        }
    }
}