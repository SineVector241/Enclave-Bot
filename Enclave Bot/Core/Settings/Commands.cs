using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Settings
{
    [Group("settings", "Setting commands.")]
    [RequireContext(ContextType.Guild)]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class Commands(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("view", "Views the server settings.")]
        public async Task ViewServerSettings()
        {
            var serverSettings = (await database.Servers
                .Include(x => x.Settings)
                .FirstAsync(x => x.Id == Context.Guild.Id)).Settings;
            
            var settings = Utils.CreateServerSettingsViewer(serverSettings, Context.User, Context.Guild);
            await RespondAsync(embed: settings.Item1, components: settings.Item2);
        }
    }
}
