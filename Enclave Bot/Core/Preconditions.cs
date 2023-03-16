using Discord.Interactions;
using Discord.WebSocket;
using Discord;

namespace Enclave_Bot.Core
{
    public class StaffOnly : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            var user = context.User;
            if (user is SocketGuildUser)
            {
                var guildUser = (SocketGuildUser)user;
                if (Database.Settings.Current.RoleSettings.StaffRole != 0 && guildUser.Roles.FirstOrDefault(x => x.Id == Database.Settings.Current.RoleSettings.StaffRole) != null || guildUser.GuildPermissions.Administrator)
                    return Task.FromResult(PreconditionResult.FromSuccess());
                else
                    return Task.FromResult(PreconditionResult.FromError("Staff Only Command!"));
            }
            else
                return Task.FromResult(PreconditionResult.FromError("Staff Only Command!"));

        }
    }

    public class DevOnly : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            if (context.User.Id == 550912080627236874)
                return Task.FromResult(PreconditionResult.FromSuccess());
            return Task.FromResult(PreconditionResult.FromError("Dev Only Command!!"));
        }
    }
}
