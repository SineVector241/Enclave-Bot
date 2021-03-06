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
        public async Task Settings([Choice("Welcome Channel", "Welcome Channel"), Choice("Logging Channel", "Logging Channel"), Choice("Application Channel", "Application Channel"), Choice("Staff Application Channel", "Staff Application Channel"), Choice("Bounty Channel", "Bounty Channel"), Choice("Suggestions Channel", "Suggestions Channel"), Choice("Verified Role", "Verified Role"), Choice("Unverified Role", "Unverified Role"), Choice("Staff Role","Staff Role"), Choice("Welcome Message", "Welcome Message"), Choice("Leave Message", "Leave Message")] string Setting = "Settings", SocketTextChannel Channel = null, IRole Role = null, [Summary(description: "When using this. you can use [user] or [guild] to display the name.")] string Message = null)
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
                        embed.AddField("Bounty Channel", settings.BountyChannel == 0 ? "Not Set" : $"<#{settings.BountyChannel}>");
                        embed.AddField("Suggestions Channel", settings.SuggestionsChannel == 0 ? "Not Set" : $"<#{settings.SuggestionsChannel}>");
                        embed.AddField("Parchment Category", settings.ParchmentCategory == 0 ? "Not Set" : $"<#{settings.ParchmentCategory}>");
                        embed.AddField("Verified Role", settings.VerifiedRole == 0 ? "Not Set" : $"<@&{settings.VerifiedRole}>");
                        embed.AddField("Unverified Role", settings.UnverifiedRole == 0 ? "Not Set" : $"<@&{settings.UnverifiedRole}>");
                        embed.AddField("Staff Role", settings.StaffRole == 0 ? "Not Set" : $"<@&{settings.StaffRole}>");
                        embed.AddField("WelcomeMessage", settings.WelcomeMessage);
                        embed.AddField("LeaveMessage", settings.LeaveMessage);
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

                    case "Bounty Channel":
                        settings.BountyChannel = Channel.Id;
                        await db.EditGuildSettings(settings);
                        embed.WithTitle($"Successfully set setting {Setting}");
                        embed.WithDescription($"Set {Setting} to <#{Channel.Id}>");
                        break;

                    case "Suggestions Channel":
                        settings.SuggestionsChannel = Channel.Id;
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

                    case "Staff Role":
                        settings.StaffRole = Role.Id;
                        await db.EditGuildSettings(settings);
                        embed.WithTitle($"Successfully set setting {Setting}");
                        embed.WithDescription($"Set {Setting} to <@&{Role.Id}>");
                        break;

                    case "Welcome Message":
                        settings.WelcomeMessage = Message;
                        await db.EditGuildSettings(settings);
                        embed.WithTitle($"Successfully set setting {Setting}");
                        embed.WithDescription($"Set {Setting} message to **{Message}**");
                        break;

                    case "Leave Message":
                        settings.LeaveMessage = Message;
                        await db.EditGuildSettings(settings);
                        embed.WithTitle($"Successfully set setting {Setting}");
                        embed.WithDescription($"Set {Setting} message to **{Message}**");
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
            try
            {
                await DeferAsync();
                SocketTextChannel channel = Context.Channel as SocketTextChannel;
                var msgs = await channel.GetMessagesAsync(limit: Amount + 1).FlattenAsync();
                await channel.DeleteMessagesAsync(msgs);
                await Context.Channel.SendMessageAsync($"Successfully cleared {Amount} messages");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var embed = new EmbedBuilder()
                    .WithTitle("An error has occured")
                    .WithDescription($"Error Message: {ex.Message}")
                    .WithColor(Color.DarkRed);
                await Context.Channel.SendMessageAsync(embed: embed.Build());
            }
        }

        [SlashCommand("kick", "Kicks a user from the guild")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser User, string Reason = "No Reason")
        {
            try
            {
                await User.KickAsync(Reason);
                await RespondAsync($"**Kicked User:** {User.Username} \n**Reason:** {Reason}");
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

        [SlashCommand("ban", "Bans a user from the guild")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser User, string Reason = "No Reason")
        {
            try
            {
                await User.BanAsync(1, Reason);
                await RespondAsync($"**Banned User:** {User.Username} \n**Reason:** {Reason}");
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

        [SlashCommand("embed", "Sends an embedded message")]
        public async Task Embed(string title, string description = null)
        {
            try
            {
                var embed = new EmbedBuilder().WithTitle(title).WithDescription(description).WithColor(utils.randomColor());
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

        [SlashCommand("emoteinfo", "Information about a custom emoji")]
        public async Task EmoteInfo(string emote)
        {
            try
            {
                var emoji = Emote.Parse(emote);
                await RespondAsync($"Emoji ID `{emote}`");
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

        [SlashCommand("activechecker", "Create, Modify, Check or Delete activity checkers")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ActivityChecker([Choice("Add", "Add"), Choice("Edit", "Edit"), Choice("View", "View"), Choice("Delete", "Delete")] string Option, SocketRole role, int Days = 1, [Choice("Notify Only", 0), Choice("Remove Roles", 1), Choice("Kick User", 2)] int Action = 0, SocketRole RemoveRole1 = null, SocketRole RemoveRole2 = null, SocketRole RemoveRole3 = null, SocketRole RemoveRole4 = null)
        {
            try
            {
                var activity = await db.HasActivity(Context.Guild.Id, role.Id);
                string action = "";
                if(Action == 0)
                    action = "Notify Only";
                if (Action == 1)
                    action = "Remove Roles";
                if (Action == 2)
                    action = "Kick User";
                switch(Option)
                {
                    case "Add":
                        if(activity)
                        {
                            await RespondAsync("This role has already been assigned to an activity");
                            return;
                        }
                        if(Action == 1 && (RemoveRole1 == null || RemoveRole2 == null || RemoveRole3 == null || RemoveRole4 == null))
                        {
                            await RespondAsync("You must fill in a RemoveRole parameter if you want to remove a role.");
                            return;
                        }
                        await db.CreateActivity(new Database.ActivityData() { ID = Context.Guild.Id, RoleID = role.Id, Action = Action, TimeDays = Days, RemoveRole1 = RemoveRole1 == null? 0 : RemoveRole1.Id, RemoveRole2 = RemoveRole2 == null ? 0 : RemoveRole2.Id, RemoveRole3 = RemoveRole3 == null ? 0 : RemoveRole3.Id, RemoveRole4 = RemoveRole4 == null ? 0 : RemoveRole4.Id });
                        var embed = new EmbedBuilder()
                            .WithTitle("Successfully created activity checker")
                            .AddField("Role", role.Mention)
                            .AddField("Days", Days)
                            .AddField("Action", action)
                            .WithColor(utils.randomColor());
                        await RespondAsync(embed: embed.Build());
                        break;
                    case "Delete":
                        if(!activity)
                        {
                            await RespondAsync("This role does not have an activity checker set on it");
                            return;
                        }
                        await db.RemoveActivity(Context.Guild.Id, role.Id);
                        await RespondAsync("Successfully removed action");
                        break;
                    case "Edit":
                        if (!activity)
                        {
                            await RespondAsync("This role does not have an activity");
                            return;
                        }
                        if (Action == 1 && (RemoveRole1 == null || RemoveRole2 == null || RemoveRole3 == null || RemoveRole4 == null))
                        {
                            await RespondAsync("You must fill in a RemoveRole parameter if you want to remove a role.");
                            return;
                        }
                        await db.UpdateActivity(new Database.ActivityData() { ID = Context.Guild.Id, RoleID = role.Id, Action = Action, TimeDays = Days, RemoveRole1 = RemoveRole1 == null ? 0 : RemoveRole1.Id, RemoveRole2 = RemoveRole2 == null ? 0 : RemoveRole2.Id, RemoveRole3 = RemoveRole3 == null ? 0 : RemoveRole3.Id, RemoveRole4 = RemoveRole4 == null ? 0 : RemoveRole4.Id });
                        var Embedded = new EmbedBuilder()
                            .WithTitle("Successfully updated activity checker")
                            .AddField("Role", role.Mention)
                            .AddField("Days", Days)
                            .AddField("Action", action)
                            .WithColor(utils.randomColor());
                        await RespondAsync(embed: Embedded.Build());
                        break;
                    case "View":
                        if (!activity)
                        {
                            await RespondAsync("This role does not have an activity");
                            return;
                        }
                        var data = await db.GetActivity(Context.Guild.Id, role.Id);
                        string actiondefine = "";
                        if (data.Action == 0)
                            actiondefine = "Notify Only";
                        if (data.Action == 1)
                            actiondefine = "Remove Roles";
                        if (data.Action == 2)
                            actiondefine = "Kick User";
                        var embed2 = new EmbedBuilder()
                            .WithTitle("Viewing Activity")
                            .AddField("Role", data.RoleID)
                            .AddField("Days", data.TimeDays.ToString())
                            .AddField("Action", actiondefine)
                            .AddField("RemoveRole1", data.RemoveRole1)
                            .AddField("RemoveRole2", data.RemoveRole2)
                            .AddField("RemoveRole3", data.RemoveRole3)
                            .AddField("RemoveRole4", data.RemoveRole4)
                            .WithColor(utils.randomColor());
                        await RespondAsync(embed: embed2.Build());
                        break;
                }
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
