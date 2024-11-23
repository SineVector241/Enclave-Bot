using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;

namespace Enclave_Bot.Core.Staff
{
    [Group("staff", "Staff commands.")]
    [RequireContext(ContextType.Guild)]
    public class Commands(DatabaseContext database, Utils utils) : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private readonly DatabaseContext Database = database;
        private readonly Utils Utils = utils;

        [SlashCommand("userinfo", "Gets a users info.")]
        public async Task UserInfo(SocketGuildUser? user = null)
        {
            user ??= Context.Guild.GetUser(Context.User.Id);
            var embed = await Utils.CreateUserInfoEmbed(user, Context.User);
            await RespondAsync(embed: embed.Build());
        }
    }
}
