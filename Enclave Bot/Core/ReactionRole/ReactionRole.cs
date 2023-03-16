using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Enclave_Bot.Core.ReactionRole
{
    public class ReactionRole : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction("RR0:*")]
        public async Task ReactionRole0(string RoleId)
        {
            var guildUser = (SocketGuildUser)Context.User;
            var role = Context.Guild.GetRole(ulong.Parse(RoleId));
            if (guildUser.Roles.FirstOrDefault(x => x.Id.ToString() == RoleId) != null && role != null)
            {
                await guildUser.RemoveRoleAsync(role);
                await RespondAsync("Removed Role Successfully", ephemeral: true);
            }
            else if(role != null)
            {
                await guildUser.AddRoleAsync(role);
                await RespondAsync("Added Role Successfully", ephemeral: true);
            }
        }

        [ComponentInteraction("RR1:*")]
        public async Task ReactionRole1(string RoleId)
        {
            var guildUser = (SocketGuildUser)Context.User;
            var role = Context.Guild.GetRole(ulong.Parse(RoleId));
            var components = ComponentBuilder.FromMessage(Context.Interaction.Message);

            if (role == null) return;

            if(guildUser.Roles.FirstOrDefault(x => x.Id == role.Id) != null)
            {
                await guildUser.RemoveRoleAsync(role);
                await RespondAsync("Removed Role Successfully", ephemeral: true);
                return;
            }

            foreach(ActionRowBuilder row in components.ActionRows)
            {
                foreach(var component in row.Components)
                {
                    if (guildUser.Roles.FirstOrDefault(x => x.Id.ToString() == component.CustomId.Replace("RR1:", "")) != null)
                    {
                        await RespondAsync($"Only 1 role is allowed to be assigned for this Reaction Role Message!\nConflicted Role: <@&{component.CustomId.Replace("RR1:", "")}>", ephemeral: true);
                        return;
                    }
                }
            }

            await guildUser.AddRoleAsync(role);
            await RespondAsync("Added Role Successfully", ephemeral: true);
        }

        [ComponentInteraction("RR2:*,*")]
        public async Task ReactionRole2(string RoleId, string RemovalRoleId)
        {
            var guildUser = (SocketGuildUser)Context.User;
            var role = Context.Guild.GetRole(ulong.Parse(RoleId));
            var rrole = Context.Guild.GetRole(ulong.Parse(RemovalRoleId));

            if (role == null || rrole == null) return;

            if(guildUser.Roles.FirstOrDefault(x => x.Id == rrole.Id) != null)
                await guildUser.RemoveRoleAsync(rrole);

            if (guildUser.Roles.FirstOrDefault(x => x.Id == role.Id) != null)
            {
                await guildUser.RemoveRoleAsync(role);
                await RespondAsync("Removed Role Successfully", ephemeral: true);
                return;
            }
            else
            {
                await guildUser.AddRoleAsync(role);
                await RespondAsync("Added Role Successfully", ephemeral: true);
                return;
            }
        }

        [ComponentInteraction("RR3:*,*")]
        public async Task ReactionRole3(string RoleId, string RemovalRoleId)
        {
            var guildUser = (SocketGuildUser)Context.User;
            var role = Context.Guild.GetRole(ulong.Parse(RoleId));
            var rrole = Context.Guild.GetRole(ulong.Parse(RemovalRoleId));

            if (role == null || rrole == null) return;

            if (guildUser.Roles.FirstOrDefault(x => x.Id == rrole.Id) == null)
            {
                await RespondAsync($"You must have the {rrole.Mention} role to use a reaction role!", ephemeral: true);
                return;
            }

            if (guildUser.Roles.FirstOrDefault(x => x.Id == role.Id) != null)
            {
                await guildUser.RemoveRoleAsync(rrole);
                await guildUser.RemoveRoleAsync(role);
                await RespondAsync("Removed Role Successfully", ephemeral: true);
                return;
            }
            else
            {
                await guildUser.RemoveRoleAsync(rrole);
                await guildUser.AddRoleAsync(role);
                await RespondAsync("Added Role Successfully", ephemeral: true);
                return;
            }
        }

        [ComponentInteraction("RR4:*,*")]
        public async Task ReactionRole4(string RoleId, string RequiredRole)
        {
            var guildUser = (SocketGuildUser)Context.User;
            var role = Context.Guild.GetRole(ulong.Parse(RoleId));
            var rrole = Context.Guild.GetRole(ulong.Parse(RequiredRole));

            if (role == null || rrole == null) return;

            if (guildUser.Roles.FirstOrDefault(x => x.Id == rrole.Id) == null)
            {
                await RespondAsync($"You must have the {rrole.Mention} role to use a reaction role!", ephemeral: true);
                return;
            }

            if (guildUser.Roles.FirstOrDefault(x => x.Id == role.Id) != null)
            {
                await guildUser.RemoveRoleAsync(role);
                await RespondAsync("Removed Role Successfully", ephemeral: true);
                return;
            }
            else
            {
                await guildUser.AddRoleAsync(role);
                await RespondAsync("Added Role Successfully", ephemeral: true);
                return;
            }
        }
    }
}
