using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Preconditions;

namespace Enclave_Bot.Core.Developer
{
    [Group("dev", "Developer commands.")]
    [IsDev]
    public class Commands(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("test", "Test the command")]
        public async Task Test()
        {
            var components = new ComponentBuilder();
            var sMenu = new SelectMenuBuilder()
                .WithType(ComponentType.RoleSelect)
                .WithCustomId("TestSMenu")
                .WithDefaultValues(SelectMenuDefaultValue.FromRole(Context.Guild.GetRole(933852857466249286)));
            components.WithSelectMenu(sMenu);
            
            await RespondAsync("Test", components: components.Build());
        }

        [SlashCommand("send-message", "Sends a message")]
        public async Task SendMessage(string message, SocketTextChannel? channel = null)
        {
            if(channel != null)
            {
                await channel.SendMessageAsync(message);
            }
            else if (Context.Channel is SocketTextChannel textChannel)
            {
                await textChannel.SendMessageAsync(message);
            }
            else
            {
                await RespondAsync("Could not send message");
                return;
            }
            await RespondAsync("Message sent", ephemeral: true);
        }
    }
}
