using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Enclave_Bot.Core.Developer
{
    [DevOnly]
    [Group("dev", "Developer Commands. Only the developer can use this!")]
    public class Commands : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("reload-settings", "Reloads the settings from settings.json file")]
        public async Task ReloadSettings()
        {
            try
            {
                Database.Settings.ReloadSettings();
                await Context.Interaction.RespondAsync("Reloaded Settings Successfully.");
            }
            catch (Exception ex)
            {
                await Context.Interaction.RespondAsync($"Could not reload settings. {ex.Message}");
            }
        }

        [SlashCommand("reload-users", "Reloads the users from users.json file")]
        public async Task ReloadUsers()
        {
            try
            {
                Database.Users.ReloadUsers();
                await Context.Interaction.RespondAsync("Reloaded Users Successfully.");
            }
            catch (Exception ex)
            {
                await Context.Interaction.RespondAsync($"Could not reload users. {ex.Message}");
            }
        }

        [SlashCommand("delete-global-commands", "Resets all global commands")]
        public async Task DeleteGlobalCommands()
        {
            await DeferAsync();
            await Context.Client.Rest.DeleteAllGlobalCommandsAsync();
            await Context.Interaction.FollowupAsync("Deleted all global commands.");
        }


        [SlashCommand("send-test-buttons", "Sends some preconfigured test buttons.")]
        public async Task SendTestButtons()
        {
            var builder = new ComponentBuilder()
                .WithButton("Send Application", "SAQ")
                .WithButton("Send Staff Application", "SSAQ")
                .Build();

            await RespondAsync(components: builder);
        }

        [SlashCommand("throw-test-error", "Throws a test error.")]
        public async Task ThrowTestError()
        {
            throw new Exception("Test Error");
        }

        [SlashCommand("send-message", "Sends a message.")]
        public async Task SendMessage(string message)
        {
            await RespondAsync(message);
        }
    }
}
