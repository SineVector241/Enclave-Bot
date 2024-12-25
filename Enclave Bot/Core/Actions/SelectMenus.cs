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
        public async Task EditServerActionGroup(string sAuthorId, string[] sActionGroupId)
        {
            try
            {
                var authorId = ulong.Parse(sAuthorId);
                var actionGroupId = long.Parse(sActionGroupId[0]);
                if (authorId != Context.User.Id)
                {
                    await RespondAsync("You are not the owner of this action group list!", ephemeral: true);
                    return;
                }

                var serverActionGroups = (await database.Servers
                    .Include(x => x.ActionsSettings)
                    .ThenInclude(x => x.ActionGroups)
                    .ThenInclude(application => application.Actions)
                    .FirstAsync(x => x.Id == Context.Guild.Id)).ActionsSettings.ActionGroups;
                var serverActionGroup = serverActionGroups.FirstOrDefault(x => x.Id == actionGroupId);

                if (serverActionGroup == null)
                {
                    //Don't really care if it fails.
                    var serverActionGroupsList = Utils.CreateServerActionGroupsList(serverActionGroups.ToArray(), 0, Context.User);
                    _ = Context.Interaction.Message.ModifyAsync(x =>
                    {
                        x.Embed = serverActionGroupsList.Item1;
                        x.Components = serverActionGroupsList.Item2;
                    });
                    await RespondAsync($"Action group with id {actionGroupId} was not found!", ephemeral: true);
                    return;
                }

                var serverActionsList = Utils.CreateServerActionsList(serverActionGroup, 0, Context.User);
                await DeferAsync();
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Embed = serverActionsList.Item1;
                    x.Components = serverActionsList.Item2;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [ComponentInteraction($"{Constants.SERVER_ACTION_GROUP_LIST_DELETE}:*")]
        public async Task DeleteServerActionGroup(string sAuthorId, string[] sActionGroupId)
        {
            var authorId = ulong.Parse(sAuthorId);
            var actionGroupId = long.Parse(sActionGroupId[0]);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this action group list!", ephemeral: true);
                return;
            }

            var serverActionGroups = (await database.Servers
                .Include(x => x.ActionsSettings)
                .ThenInclude(x => x.ActionGroups)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ActionsSettings.ActionGroups;
            var serverActionGroup = serverActionGroups.FirstOrDefault(x => x.Id == actionGroupId);

            if (serverActionGroup == null)
            {
                //Don't really care if it fails.
                var serverActionGroupsList = Utils.CreateServerActionGroupsList(serverActionGroups.ToArray(), 0, Context.User);
                _ = Context.Interaction.Message.ModifyAsync(x =>
                {
                    x.Embed = serverActionGroupsList.Item1;
                    x.Components = serverActionGroupsList.Item2;
                });
                await RespondAsync($"Action group with id {actionGroupId} was not found!", ephemeral: true);
                return;
            }

            database.Remove(serverActionGroup);
            await database.SaveChangesAsync();
            await RespondAsync($"Action group with id {actionGroupId} was deleted!", ephemeral: true);

            //Don't really care if it fails.
            var serverActionGroupsList2 = Utils.CreateServerActionGroupsList(serverActionGroups.ToArray(), 0, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = serverActionGroupsList2.Item1;
                x.Components = serverActionGroupsList2.Item2;
            });
        }
        
        [ComponentInteraction($"{Constants.SERVER_ACTION_LIST_EDIT}:*,*")]
        public async Task EditServerAction(string sAuthorId, string sActionGroupId, string[] sActionId)
        {
            try
            {
                var authorId = ulong.Parse(sAuthorId);
                var actionGroupId = long.Parse(sActionGroupId);
                var actionId = long.Parse(sActionId[0]);
                if (authorId != Context.User.Id)
                {
                    await RespondAsync("You are not the owner of this action group list!", ephemeral: true);
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

                var serverAction = serverActionGroup.Actions.FirstOrDefault(x => x.Id == actionId);
                if (serverAction == null)
                {
                    var actionsList = Utils.CreateServerActionsList(serverActionGroup, 0, Context.User);
                    await ModifyOriginalResponseAsync(x =>
                    {
                        x.Embed = actionsList.Item1;
                        x.Components = actionsList.Item2;
                    });
                    await RespondAsync($"Action with id {actionId} was not found!", ephemeral: true);
                    return;
                }

                var actionEditor = Utils.CreateServerActionEditor(serverAction, Context.User, Context.Guild);
                await DeferAsync();
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Embed = actionEditor.Item1;
                    x.Components = actionEditor.Item2;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [ComponentInteraction($"{Constants.SERVER_ACTION_LIST_DELETE}:*,*")]
        public async Task DeleteServerAction(string sAuthorId, string sActionGroupId, string[] sActionId)
        {
            var authorId = ulong.Parse(sAuthorId);
            var actionGroupId = long.Parse(sActionGroupId);
            var actionId = long.Parse(sActionId[0]);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this action group list!", ephemeral: true);
                return;
            }

            var serverActionGroup = (await database.Servers
                .Include(x => x.ActionsSettings)
                .ThenInclude(x => x.ActionGroups)
                .ThenInclude(application => application.Actions)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ActionsSettings.ActionGroups.FirstOrDefault(x => x.Id == actionGroupId);

            if (serverActionGroup == null)
            {
                await RespondAsync($"Action group with id {actionGroupId} was not found!", ephemeral: true);
                return;
            }
            
            var action = serverActionGroup.Actions.FirstOrDefault(x => x.Id == actionId);
            if (action == null)
            {
                var actionsList = Utils.CreateServerActionsList(serverActionGroup, 0, Context.User);
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Embed = actionsList.Item1;
                    x.Components = actionsList.Item2;
                });
                await RespondAsync($"Action with id {actionId} was not found!", ephemeral: true);
                return;
            }
            
            serverActionGroup.Actions.Remove(action);
            await database.SaveChangesAsync();
            await RespondAsync($"Action with id {actionId} was deleted!", ephemeral: true);
            
            //Don't really care if it fails.
            var actionsList2 = Utils.CreateServerActionsList(serverActionGroup, 0, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = actionsList2.Item1;
                x.Components = actionsList2.Item2;
            });
        }
    }
}