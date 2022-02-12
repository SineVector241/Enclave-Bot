using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using Fergun.Interactive;

namespace Enclave_Bot.Core.SlashCommands
{
    public class ModerationCommands : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private Database.Database db = new Database.Database();
        private Utils utils = new Utils();
        public InteractiveService Interactive { get; set; }

        [SlashCommand("settings", "Gets or sets the bot settings for this guild")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Settings([Choice("Welcome Channel", "Welcome Channel"), Choice("Logging Channel", "Logging Channel"), Choice("Application Channel", "Application Channel"), Choice("Staff Application Channel", "Staff Application Channel"),Choice("Verified Role", "Verified Role"), Choice("Unverified Role", "Unverified Role")] string Setting = "Settings", [ChannelTypes(ChannelType.Text)] IGuildChannel Channel = null, IRole Role = null)
        {
            try
            {
                var settings = await db.GetGuildSettingsById(Context.Guild.Id);
                var embed = new EmbedBuilder()
                    .WithColor(utils.randomColor());
                switch (Setting)
                {
                    case "Settings":
                        embed.WithTitle("Guild Settings");
                        embed.AddField("Welcome Channel", settings.WelcomeChannel == 0 ? "Not Set" : $"<#{settings.WelcomeChannel}>");
                        embed.AddField("Logging Channel", settings.LoggingChannel == 0 ? "Not Set" : $"<#{settings.LoggingChannel}>");
                        embed.AddField("Application Channel", settings.ApplicationChannel == 0 ? "Not Set" : $"<#{settings.ApplicationChannel}>");
                        embed.AddField("Staff Application Channel", settings.StaffApplicationChannel == 0 ? "Not Set" : $"<#{settings.StaffApplicationChannel}>");
                        embed.AddField("Parchment Category", settings.ParchmentCategory == 0 ? "Not Set" : $"<#{settings.ParchmentCategory}>");
                        embed.AddField("Verified Role", settings.VerifiedRole == 0 ? "Not Set" : $"<@&{settings.VerifiedRole}>");
                        embed.AddField("Unverified Role", settings.UnverifiedRole == 0 ? "Not Set" : $"<@&{settings.UnverifiedRole}>");
                        break;

                    case "Welcome Channel":
                        settings.WelcomeChannel = Channel.Id;
                        await db.EditGuildSettings(settings);
                        embed.WithTitle($"Successfully set setting {Setting}");
                        embed.WithDescription($"Set {Setting} to <#{Channel.Id}>");
                        break;

                    case "Logging Channel":
                        settings.LoggingChannel = Channel.Id;
                        await db.EditGuildSettings(settings);
                        embed.WithTitle($"Successfully set setting {Setting}");
                        embed.WithDescription($"Set {Setting} to <#{Channel.Id}>");
                        break;

                    case "Application Channel":
                        settings.ApplicationChannel = Channel.Id;
                        await db.EditGuildSettings(settings);
                        embed.WithTitle($"Successfully set setting {Setting}");
                        embed.WithDescription($"Set {Setting} to <#{Channel.Id}>");
                        break;

                    case "Staff Application Channel":
                        settings.StaffApplicationChannel = Channel.Id;
                        await db.EditGuildSettings(settings);
                        embed.WithTitle($"Successfully set setting {Setting}");
                        embed.WithDescription($"Set {Setting} to <#{Channel.Id}>");
                        break;

                    case "Unverified Role":
                        settings.UnverifiedRole = Role.Id;
                        await db.EditGuildSettings(settings);
                        embed.WithTitle($"Successfully set setting {Setting}");
                        embed.WithDescription($"Set {Setting} to <@&{Role.Id}>");
                        break;

                    case "Verified Role":
                        settings.VerifiedRole = Role.Id;
                        await db.EditGuildSettings(settings);
                        embed.WithTitle($"Successfully set setting {Setting}");
                        embed.WithDescription($"Set {Setting} to <@&{Role.Id}>");
                        break;
                }
                await RespondAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var embed = new EmbedBuilder()
                    .WithTitle("An error has occured")
                    .WithDescription($"Error Message: {ex.Message}")
                    .WithColor(Color.DarkRed);
                await RespondAsync(embed: embed.Build());
            }
        }
    }
}
