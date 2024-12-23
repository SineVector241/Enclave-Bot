using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Preconditions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Actions
{
    [IsStaff]
    public class SelectMenus(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction($"{Constants.SERVER_ACTION_GROUP_LIST_EDIT}:*")]
        public async Task EditActionGroup(string sAuthorId, string[] sActionGroupId)
        {
            var authorId = ulong.Parse(sAuthorId);
            var actionGroupId = Guid.Parse(sActionGroupId[0]);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this action group list!", ephemeral: true);
                return;
            }

            var actionGroups = (await database.Servers
                .Include(x => x.ActionsSettings)
                .ThenInclude(x => x.ActionGroups)
                .ThenInclude(application => application.Actions)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ActionsSettings.ActionGroups;
            var actionGroup = actionGroups.FirstOrDefault(x => x.Id == actionGroupId);

            if (actionGroup == null)
            {
                //Don't really care if it fails.
                var actionGroupsList = Utils.CreateActionGroupsList(actionGroups.ToArray(), 0, Context.User);
                _ = Context.Interaction.Message.ModifyAsync(x =>
                {
                    x.Embed = actionGroupsList.Item1;
                    x.Components = actionGroupsList.Item2;
                });
                await RespondAsync($"Action group with id {actionGroupId} was not found!", ephemeral: true);
                return;
            }
            
            var actionsList = Utils.CreateActionsList(actionGroup, 0, Context.User);
            await DeferAsync();
            await ModifyOriginalResponseAsync(x =>
            {
                x.Embed = actionsList.Item1;
                x.Components = actionsList.Item2;
            });
        }

        [ComponentInteraction($"{Constants.SERVER_ACTION_GROUP_LIST_DELETE}:*")]
        public async Task DeleteActionGroup(string sAuthorId, string[] sActionGroupId)
        {
            var authorId = ulong.Parse(sAuthorId);
            var actionGroupId = Guid.Parse(sActionGroupId[0]);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this action group list!", ephemeral: true);
                return;
            }

            var actionGroups = (await database.Servers
                .Include(x => x.ActionsSettings)
                .ThenInclude(x => x.ActionGroups)
                .ThenInclude(application => application.Actions)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ActionsSettings.ActionGroups;
            var actionGroup = actionGroups.FirstOrDefault(x => x.Id == actionGroupId);

            if (actionGroup == null)
            {
                //Don't really care if it fails.
                var actionGroupsList = Utils.CreateActionGroupsList(actionGroups.ToArray(), 0, Context.User);
                _ = Context.Interaction.Message.ModifyAsync(x =>
                {
                    x.Embed = actionGroupsList.Item1;
                    x.Components = actionGroupsList.Item2;
                });
                await RespondAsync($"Action group with id {actionGroupId} was not found!", ephemeral: true);
                return;
            }

            database.Remove(actionGroup);
            await database.SaveChangesAsync();
            await RespondAsync($"Action group with id {actionGroupId} was deleted!", ephemeral: true);

            //Don't really care if it fails.
            var actionGroupsList2 = Utils.CreateActionGroupsList(actionGroups.ToArray(), 0, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = actionGroupsList2.Item1;
                x.Components = actionGroupsList2.Item2;
            });
        }
    }
}