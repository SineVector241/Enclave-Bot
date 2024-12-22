using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Settings
{
    [RequireUserPermission(GuildPermission.Administrator)]
    public class SelectMenus(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction($"{Constants.SERVER_SETTINGS_SET_STAFF_ROLES}:*")]
        public async Task SetStaffRoles(string sAuthorId, IRole[] roles)
        {
            var authorId = ulong.Parse(sAuthorId);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this application list!", ephemeral: true);
                return;
            }
            
            var serverSettings = (await database.Servers
                .Include(x => x.Settings)
                .FirstAsync(x => x.Id == Context.Guild.Id)).Settings;
            serverSettings.StaffRoles = roles.Select(x => x.Id).ToList();
            
            await database.SaveChangesAsync();
            await RespondAsync($"Successfully set staff roles!", ephemeral: true);
            
            //Don't really care if it fails.
            var serverSettingsViewer = Utils.CreateServerSettingsViewer(serverSettings, Context.User, Context.Guild);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = serverSettingsViewer.Item1;
                x.Components = serverSettingsViewer.Item2;
            });
        }
    }
}