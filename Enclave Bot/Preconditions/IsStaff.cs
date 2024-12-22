using Discord;
using Discord.Interactions;
using Enclave_Bot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Enclave_Bot.Preconditions
{
    public class IsStaff : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            if (context.User is not IGuildUser guildUser) return Task.FromResult(PreconditionResult.FromError("Command must be used in a guild channel."));
            if (guildUser.GuildPermissions.Administrator) return Task.FromResult(PreconditionResult.FromSuccess());
            var database = services.GetRequiredService<DatabaseContext>();
            var serverSettings = database.Servers
                .Include(x => x.Settings)
                .FirstAsync(x => x.Id == guildUser.Guild.Id).GetAwaiter().GetResult().Settings;
            var validRoles = serverSettings.StaffRoles.Where(x => guildUser.Guild.GetRole(x) != null);
            return Task.FromResult(validRoles.Any(role => guildUser.RoleIds.Contains(role))
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError("User is not staff!"));
        }
    }
}