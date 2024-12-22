using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Preconditions;

namespace Enclave_Bot.Core.Staff
{
    [Group("staff", "Staff commands.")]
    [RequireContext(ContextType.Guild)]
    [IsStaff]
    public class Commands(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("userinfo", "Gets a users info.")]
        public async Task UserInfo(SocketGuildUser? user = null)
        {
            user ??= Context.Guild.GetUser(Context.User.Id);
            await database.CreateUserIfNotExistsAsync(user);
            var botUser = database.Users.First(x => x.Id == user.Id);
            var embed = Utils.CreateUserInfo(user, botUser, Context.User);
            await RespondAsync(embed: embed);
        }
    }
}
