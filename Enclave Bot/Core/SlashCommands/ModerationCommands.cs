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
        public async Task Settings([Choice("Welcome Channel", "Welcome Channel"), Choice("Logging Channel", "Logging Channel"), Choice("Application Channel", "Application Channel"), Choice("Staff Application Channel", "Staff Application Channel"), Choice("Verified Role", "Verified Role"), Choice("Unverified Role", "Unverified Role")] string Setting = "Settings", [ChannelTypes(ChannelType.Text)] IGuildChannel Channel = null, IRole Role = null)
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

        [SlashCommand("rrcreate", "Creates reaction role message")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ReactionRoleCreate(SocketTextChannel Channel, [Choice("Normal", "RR1"), Choice("1 Role Only", "RR2"), Choice("Add 1 Role Remove 1 Role", "RR3")] string Type, IRole Role1, IRole Role2 = null, IRole Role3 = null, IRole Role4 = null, IRole Role5 = null, IRole RemovalRole = null, string Title = "Reaction Roles", string Description = null)
        {
            try
            {
                if (RemovalRole == null && Type == "RR3")
                {
                    await RespondAsync("Please select a RemovalRole");
                    return;
                }
                else if (RemovalRole != null && Type == "RR3")
                {
                    var builder = new ComponentBuilder()
                        .WithButton(Role1.Name, $"{Type}:{Role1.Id},{RemovalRole.Id}");
                    if (Role2 != null)
                        builder.WithButton(Role2.Name, $"{Type}:{Role2.Id},{RemovalRole.Id}");
                    if (Role3 != null)
                        builder.WithButton(Role3.Name, $"{Type}:{Role3.Id},{RemovalRole.Id}");
                    if (Role4 != null)
                        builder.WithButton(Role4.Name, $"{Type}:{Role4.Id},{RemovalRole.Id}");
                    if (Role5 != null)
                        builder.WithButton(Role5.Name, $"{Type}:{Role5.Id},{RemovalRole.Id}");
                    await Channel.SendMessageAsync(embed: new EmbedBuilder().WithTitle(Title).WithDescription(Description).WithColor(utils.randomColor()).Build(), components: builder.Build());
                }
                else if (Type != "RR3")
                {
                    var builder = new ComponentBuilder()
                        .WithButton(Role1.Name, $"{Type}:{Role1.Id}");
                    if (Role2 != null)
                        builder.WithButton(Role2.Name, $"{Type}:{Role2.Id}");
                    if (Role3 != null)
                        builder.WithButton(Role3.Name, $"{Type}:{Role3.Id}");
                    if (Role4 != null)
                        builder.WithButton(Role4.Name, $"{Type}:{Role4.Id}");
                    if (Role5 != null)
                        builder.WithButton(Role5.Name, $"{Type}:{Role5.Id}");
                    await Channel.SendMessageAsync(embed: new EmbedBuilder().WithTitle(Title).WithDescription(Description).WithColor(utils.randomColor()).Build(), components: builder.Build());
                }
                await RespondAsync(embed: new EmbedBuilder().WithColor(utils.randomColor()).WithTitle("Sent Reaction Role").Build());
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

        [SlashCommand("purge", "Clears a certain amount of messages in a channel")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Purge(int Amount)
        {
            SocketTextChannel channel = Context.Channel as SocketTextChannel;
            var msgs = await channel.GetMessagesAsync(limit: Amount).FlattenAsync();
            await channel.DeleteMessagesAsync(msgs);
            await RespondAsync($"Successfully cleared {Amount} messages");
        }

        [SlashCommand("kick", "Kicks a user from the guild")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser User, string Reason = "No Reason")
        {
            await User.KickAsync(Reason);
            await RespondAsync($"**Kicked User:** {User.Username} \n**Reason:** {Reason}");
        }

        [SlashCommand("ban","Bans a user")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser User, string Reason = "No Reason")
        {
            await User.BanAsync(1, Reason);
            await RespondAsync($"**Banned User:** {User.Username} \n**Reason:** {Reason}");
        }
    }
}
