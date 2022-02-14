using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;

namespace Enclave_Bot.Core.Interactions
{
    public class ReactionRoleInteractions : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        public InteractiveService Interactive { get; set; }

        [ComponentInteraction("RR1:*")]
        public async Task ReactionRole(string roleID)
        {
            SocketGuildUser? user = Context.User as SocketGuildUser;
            var role = Context.Guild.GetRole((ulong)Convert.ToInt64(roleID));
            if (user.Roles.Contains(role))
            {
                await user.RemoveRoleAsync(role);
                await RespondAsync("Successfully Removed Role", ephemeral: true);
            }
            else
            {
                await user.AddRoleAsync(role);
                await RespondAsync("Successfully Added Role", ephemeral: true);
            }
        }

        [ComponentInteraction("RR2:*")]
        public async Task ReactionRole1Role(string roleID)
        {
            var components = Context.Interaction.Message.Components.First().Components;
            var buttons = components.Where(x => x.Type is ComponentType.Button).ToList();
            var user = Context.User as SocketGuildUser;
            foreach(var button in buttons)
            {
                var role = Context.Guild.GetRole((ulong)Convert.ToInt64(button.CustomId.Replace("RR2:", "")));
                if(user.Roles.Contains(role) && role.Id.ToString() == roleID)
                {
                    await user.RemoveRoleAsync((ulong)Convert.ToInt64(roleID));
                    await RespondAsync("Successfully Removed Role", ephemeral: true);
                    return;
                }
                if (user.Roles.Contains(role))
                {
                    await RespondAsync("You already have another role from this reaction role message. Please remove it before assigning a new one",ephemeral: true);
                    return;
                }
            }

            await user.AddRoleAsync((ulong)Convert.ToInt64(roleID));
            await RespondAsync("Successfully Added Role", ephemeral: true);
        }

        [ComponentInteraction("RR3:*,*")]
        public async Task AddAndRemoveRoles(string roleID, string roleRemoveID)
        {
            var user = Context.User as SocketGuildUser;
            if (user.Roles.Contains(Context.Guild.GetRole((ulong)Convert.ToInt64(roleRemoveID))))
            {
                await user.RemoveRoleAsync((ulong)Convert.ToInt64(roleRemoveID));
                await user.AddRoleAsync((ulong)Convert.ToInt64(roleID));
                await RespondAsync("Successfully Added Role", ephemeral: true);
                return;
            }

            await RespondAsync($"You do not have the required role <@&{roleRemoveID}> to use this reaction panel", ephemeral: true);
        }
    }
}