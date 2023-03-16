using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Enclave_Bot.Core.Settings
{
    [StaffOnly]
    [Group("settings", "Settings for the bot.")]
    public class Commands : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [Group("role", "Role Settings")]
        public class RoleSettings : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
        {
            [SlashCommand("view", "View the settings")]
            public async Task View()
            {
                var settings = Database.Settings.Current.RoleSettings;
                var staffAcceptRoles = "";

                for (int i = 0; i < settings.StaffAcceptRoles.Count; i++)
                {
                    var role = Context.Guild.GetRole(settings.StaffAcceptRoles[i]);
                    staffAcceptRoles += role.Mention + "\n";
                }

                var embed = new EmbedBuilder()
                    .WithTitle("Role Settings")
                    .AddField("Unverified Role", settings.UnverifiedRole == 0 ? "Not Set" : $"<@&{settings.UnverifiedRole}>")
                    .AddField("Verified Role", settings.VerifiedRole == 0 ? "Not Set" : $"<@&{settings.VerifiedRole}>")
                    .AddField("Staff Role", settings.StaffRole == 0 ? "Not Set" : $"<@&{settings.StaffRole}>")
                    .AddField("Staff Accept Roles", staffAcceptRoles == ""? "None Set" : staffAcceptRoles)
                    .WithColor(Utils.RandomColor())
                    .Build();

                await RespondAsync(embed: embed);
            }

            [SlashCommand("set-unverified-role", "Sets the role for the Unverified Role Setting.")]
            public async Task SetUnverifiedRole(SocketRole role)
            {
                var settings = Database.Settings.Current.RoleSettings;
                settings.UnverifiedRole = role.Id;
                Database.Settings.UpdateSettings();

                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("set-verified-role", "Sets the role for the Verified Role Setting.")]
            public async Task SetVerifiedRole(SocketRole role)
            {
                var settings = Database.Settings.Current.RoleSettings;
                settings.VerifiedRole = role.Id;
                Database.Settings.UpdateSettings();

                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("set-staff-role", "Sets the role for the Staff Role Setting.")]
            public async Task SetStaffRole(SocketRole role)
            {
                var settings = Database.Settings.Current.RoleSettings;
                settings.StaffRole = role.Id;
                Database.Settings.UpdateSettings();

                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("add-accept-role", "Adds staff accept role for the Staff Accept Roles Setting.")]
            public async Task AddStaffAcceptRole(SocketRole role)
            {
                var settings = Database.Settings.Current.RoleSettings;
                settings.StaffAcceptRoles.Add(role.Id);
                Database.Settings.UpdateSettings();

                await RespondAsync("Added role successfully", ephemeral: true);
            }

            [SlashCommand("remove-accept-role", "Removes staff accept role from the Staff Accept Roles Setting.")]
            public async Task RemoveStaffAcceptRole(SocketRole role)
            {
                var settings = Database.Settings.Current.RoleSettings;
                if(settings.StaffAcceptRoles.Exists(x => x == role.Id))
                {
                    settings.StaffAcceptRoles.Remove(role.Id);
                }
                Database.Settings.UpdateSettings();

                await RespondAsync("Removed role successfully", ephemeral: true);
            }
        }


        [Group("application", "Application Settings")]
        public class ApplicationSettings : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
        {
            [SlashCommand("view", "View the settings.")]
            public async Task View()
            {
                var settings = Database.Settings.Current.ApplicationSettings;
                var appQuestions = "";
                var sAppQuestions = "";

                for(int i = 0; i < settings.ApplicationQuestions.Count; i++)
                {
                    appQuestions += $"**{i+1}:** {settings.ApplicationQuestions[i]}\n";
                }
                for (int i = 0; i < settings.StaffApplicationQuestions.Count; i++)
                {
                    sAppQuestions += $"**{i+1}:** {settings.StaffApplicationQuestions[i]}\n";
                }

                var embed = new EmbedBuilder()
                    .WithTitle("Application Settings")
                    .AddField("Kick After # Of Denials", settings.KickAfterDenials == 0 ? "Infinite" : settings.KickAfterDenials)
                    .AddField("Application Channel", settings.ApplicationChannel == 0 ? "Not Set" : $"<#{settings.ApplicationChannel}>")
                    .AddField("Staff Application Channel", settings.StaffApplicationChannel == 0 ? "Not Set" : $"<#{settings.StaffApplicationChannel}>")
                    .AddField("Application Questions", string.IsNullOrWhiteSpace(appQuestions) ? "No Questions Set" : appQuestions)
                    .AddField("Staff Application Questions", string.IsNullOrWhiteSpace(sAppQuestions) ? "No Questions Set" : sAppQuestions)
                    .WithColor(Utils.RandomColor())
                    .Build();

                await RespondAsync(embed: embed);
            }

            [SlashCommand("set-kick-after-denials", "Sets the number for the Kick After # Of Denials Setting.")]
            public async Task SetKickAfterDenials([Summary(description: "Set to 0 to disable kick after denials.")]int amount)
            {
                var settings = Database.Settings.Current.ApplicationSettings;
                settings.KickAfterDenials = amount;
                Database.Settings.UpdateSettings();

                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("set-app-channel", "Sets the channel for the Application Channel Setting.")]
            public async Task SetApplicationChannel(SocketTextChannel channel)
            {
                var settings = Database.Settings.Current.ApplicationSettings;
                settings.ApplicationChannel = channel.Id;
                Database.Settings.UpdateSettings();

                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("set-staffapp-channel", "Sets the channel for the Staff Application Channel Setting.")]
            public async Task SetStaffApplicationChannel(SocketTextChannel channel)
            {
                var settings = Database.Settings.Current.ApplicationSettings;
                settings.StaffApplicationChannel = channel.Id;
                Database.Settings.UpdateSettings();

                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("add-app-question", "Adds a question for the Application Questions Setting.")]
            public async Task AddApplicationQuestion(string question)
            {
                var settings = Database.Settings.Current.ApplicationSettings;
                if (settings.ApplicationQuestions.Count >= 25)
                {
                    await RespondAsync("Error. Can only have 25 questions maximum");
                    return;
                }

                settings.ApplicationQuestions.Add(question);
                Database.Settings.UpdateSettings();

                await RespondAsync("Added question successfully", ephemeral: true);
            }

            [SlashCommand("add-staffapp-question", "Adds a question for the Staff Application Questions Setting.")]
            public async Task AddStaffApplicationQuestion(string question)
            {
                var settings = Database.Settings.Current.ApplicationSettings;
                if(settings.StaffApplicationQuestions.Count >= 25)
                {
                    await RespondAsync("Error. Can only have 25 questions maximum");
                    return;
                }

                settings.StaffApplicationQuestions.Add(question);
                Database.Settings.UpdateSettings();

                await RespondAsync("Added question successfully", ephemeral: true);
            }

            [SlashCommand("remove-app-question", "Removes a question for the Application Questions Setting.")]
            public async Task RemoveApplicationQuestion(int index)
            {
                var settings = Database.Settings.Current.ApplicationSettings;
                settings.ApplicationQuestions.RemoveAt(index - 1);
                Database.Settings.UpdateSettings();

                await RespondAsync("Removed question successfully", ephemeral: true);
            }

            [SlashCommand("remove-staffapp-question", "Removes a question for the Staff Application Questions Setting.")]
            public async Task RemoveStaffApplicationQuestion(int index)
            {
                var settings = Database.Settings.Current.ApplicationSettings;
                settings.StaffApplicationQuestions.RemoveAt(index - 1);
                Database.Settings.UpdateSettings();

                await RespondAsync("Removed question successfully", ephemeral: true);
            }
        }


        [Group("greeting", "Greeting Settings")]
        public class GreetingSettings : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
        {
            [SlashCommand("view", "View the settings.")]
            public async Task View()
            {
                var settings = Database.Settings.Current.GreetingSettings;
                var embed = new EmbedBuilder()
                    .WithTitle("Greeting Settings")
                    .AddField("Send Join Message", settings.SendJoinMessage? ":white_check_mark:" : ":x:")
                    .AddField("Send Leave Message", settings.SendLeaveMessage? ":white_check_mark:" : ":x:")
                    .AddField("Show Profile In Thumbnail", settings.ShowProfileInThumbnail? ":white_check_mark:" : ":x:")
                    .AddField("Show Profile In Picture", settings.ShowProfileInPicture? ":white_check_mark:" : ":x:")
                    .AddField("Mentions User", settings.MentionUser? ":white_check_mark:" : ":x:")
                    .AddField("Adds Join Role", settings.AddJoinRole? ":white_check_mark:" : ":x:")
                    .AddField("Join Message Title", settings.JoinMessageTitle)
                    .AddField("Leave Message Title", settings.LeaveMessageTitle)
                    .AddField("Join Message", settings.JoinMessage)
                    .AddField("Leave Message", settings.LeaveMessage)
                    .AddField("Role On Join", settings.JoinRole == 0? "Not Set" : $"<@&{settings.JoinRole}>")
                    .AddField("Greeting Channel", settings.GreetingChannel == 0? "Not Set" : $"<#{settings.GreetingChannel}>")
                    .WithColor(Utils.RandomColor())
                    .Build();

                Database.Settings.UpdateSettings();
                await RespondAsync(embed: embed);
            }

            [SlashCommand("set-toggle-value", "Sets a setting on or off.")]
            public async Task SetToggleValue([
                Choice("Send Join Message", 0),
                Choice("Send Leave Message", 1),
                Choice("Show Profile In Thumbnail", 2),
                Choice("Show Profile In Picture", 3),
                Choice("Mentions User", 4),
                Choice("Adds Join Role", 5)] int toggle, bool value)
            {
                var settings = Database.Settings.Current.GreetingSettings;
                switch(toggle)
                {
                    case 0: settings.SendJoinMessage = value;
                        break;
                    case 1:
                        settings.SendLeaveMessage = value;
                        break;
                    case 2:
                        settings.ShowProfileInThumbnail = value;
                        break;
                    case 3:
                        settings.ShowProfileInPicture = value;
                        break;
                    case 4:
                        settings.MentionUser = value;
                        break;
                    case 5:
                        settings.AddJoinRole = value;
                        break;
                }

                Database.Settings.UpdateSettings();
                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("set-join-message-title", "Sets the message for the Join Message Title Setting.")]
            public async Task SetJoinMessageTitle(string message)
            {
                var settings = Database.Settings.Current.GreetingSettings;
                settings.JoinMessageTitle = message;

                Database.Settings.UpdateSettings();
                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("set-leave-message-title", "Sets the message for the Leave Message Title Setting.")]
            public async Task SetLeaveMessageTitle(string message)
            {
                var settings = Database.Settings.Current.GreetingSettings;
                settings.LeaveMessageTitle = message;

                Database.Settings.UpdateSettings();
                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("set-join-message", "Sets the message for the Join Message Setting.")]
            public async Task SetJoinMessage(string message)
            {
                var settings = Database.Settings.Current.GreetingSettings;
                settings.JoinMessage = message;

                Database.Settings.UpdateSettings();
                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("set-leave-message", "Sets the message for the Leave Message Setting.")]
            public async Task SetLeaveMessage(string message)
            {
                var settings = Database.Settings.Current.GreetingSettings;
                settings.LeaveMessage = message;

                Database.Settings.UpdateSettings();
                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("set-join-role", "Sets the role for the Role On Join Setting.")]
            public async Task SetJoinRole(SocketRole role)
            {
                var settings = Database.Settings.Current.GreetingSettings;
                settings.JoinRole = role.Id;

                Database.Settings.UpdateSettings();
                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("set-greeting-channel", "Sets the channel for the Greeting Channel Setting.")]
            public async Task SetGreetingChannel(SocketTextChannel channel)
            {
                var settings = Database.Settings.Current.GreetingSettings;
                settings.GreetingChannel = channel.Id;

                Database.Settings.UpdateSettings();
                await RespondAsync("Setting successfully set", ephemeral: true);
            }
        }

        [Group("miscellaneous", "Miscellaneous Settings")]
        public class MiscellaneousSettings : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
        {
            [SlashCommand("view", "View the settings.")]
            public async Task View()
            {
                var settings = Database.Settings.Current.MiscellaneousSettings;
                var embed = new EmbedBuilder()
                    .WithTitle("Miscellanous Settings")
                    .AddField("Bounty Channel", settings.BountyChannel == 0 ? "Not Set" : $"<#{settings.BountyChannel}>")
                    .AddField("Suggestions Channel", settings.SuggestionsChannel == 0? "Not Set" : $"<#{settings.SuggestionsChannel}>")
                    .WithColor(Utils.RandomColor())
                    .Build();

                await RespondAsync(embed: embed);
            }

            [SlashCommand("set-bounty-channel", "Sets the channel for the Bounty Channel Setting.")]
            public async Task SetBountyChannel(SocketTextChannel channel)
            {
                var settings = Database.Settings.Current.MiscellaneousSettings;
                settings.BountyChannel = channel.Id;

                Database.Settings.UpdateSettings();
                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("set-suggestions-channel", "Sets the channel for the Suggestions Channel Setting.")]
            public async Task SetSuggestionsChannel(SocketTextChannel channel)
            {
                var settings = Database.Settings.Current.MiscellaneousSettings;
                settings.SuggestionsChannel = channel.Id;

                Database.Settings.UpdateSettings();
                await RespondAsync("Setting successfully set", ephemeral: true);
            }
        }

        [Group("logging", "Logging Settings")]
        public class LoggingSettings : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
        {
            [SlashCommand("view", "View the settings.")]
            public async Task View()
            {
                var settings = Database.Settings.Current.LoggingSettings;
                var embed = new EmbedBuilder()
                    .WithTitle("Logging Settings")
                    .AddField("Logging Channel", settings.LoggingChannel == 0 ? "Not Set" : $"<#{settings.LoggingChannel}>")
                    .AddField("Logging Enabled", settings.LoggingEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("Message Edited Enabled", settings.MessageEditedEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("Message Deleted Enabled", settings.MessageDeletedEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("Channel Updated Enabled", settings.ChannelUpdatedEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("Channel Deleted Enabled", settings.ChannelDeletedEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("Channel Created Enabled", settings.ChannelCreatedEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("Joined Voice Enabled", settings.JoinedVoiceEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("Left Voice Enabled", settings.LeftVoiceEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("Moved Voice Enabled", settings.MovedVoiceEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("User Updated Enabled", settings.UserUpdatedEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("User Joined Enabled", settings.UserJoinedEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("User Left Enabled", settings.UserLeftEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("User Banned Enabled", settings.UserBannedEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("User Unbanned Enabled", settings.UserUnbannedEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("Invite Created Enabled", settings.InviteCreatedEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("Invite Deleted Enabled", settings.InviteDeletedEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("Role Created Enabled", settings.RoleCreatedEnabled ? ":white_check_mark:" : ":x:")
                    .AddField("Role Deleted Enabled", settings.RoleDeletedEnabled ? ":white_check_mark:" : ":x:")
                    .WithColor(Utils.RandomColor())
                    .Build();

                await RespondAsync(embed: embed);
            }

            [SlashCommand("set-logging-channel", "Sets the channel for the Logging Channel Setting.")]
            public async Task SetLoggingChannel(SocketTextChannel channel)
            {
                var settings = Database.Settings.Current.LoggingSettings;
                settings.LoggingChannel = channel.Id;

                Database.Settings.UpdateSettings();
                await RespondAsync("Setting successfully set", ephemeral: true);
            }

            [SlashCommand("set-log-enabled-value", "Sets a logging setting on or off.")]
            public async Task SetLogEnabledValue([
                Choice("Logging Enabled", 0),
                Choice("Message Edited Enabled", 1),
                Choice("Message Deleted Enabled", 2),
                Choice("Channel Updated Enabled", 3),
                Choice("Channel Deleted Enabled", 4),
                Choice("Channel Created Enabled", 5),
                Choice("Joined Voice Enabled", 6),
                Choice("Left Voice Enabled", 7),
                Choice("Moved Voice Enabled", 8),
                Choice("User Updated Enabled", 9),
                Choice("User Joined Enabled", 10),
                Choice("User Left Enabled", 11),
                Choice("User Banned Enabled", 12),
                Choice("User Unbanned Enabled", 13),
                Choice("Invite Created Enabled", 14),
                Choice("Invite Deleted Enabled", 15),
                Choice("Role Created Enabled", 16),
                Choice("Role Deleted Enabled", 17)]int toggle, bool value)
            {
                var settings = Database.Settings.Current.LoggingSettings;
                switch (toggle)
                {
                    case 0:
                        settings.LoggingEnabled = value;
                        break;
                    case 1:
                        settings.MessageEditedEnabled = value;
                        break;
                    case 2: 
                        settings.MessageDeletedEnabled = value;
                        break;
                    case 3:
                        settings.ChannelUpdatedEnabled = value;
                        break;
                    case 4:
                        settings.ChannelDeletedEnabled = value;
                        break;
                    case 5:
                        settings.ChannelCreatedEnabled = value;
                        break;
                    case 6:
                        settings.JoinedVoiceEnabled = value;
                        break;
                    case 7:
                        settings.LeftVoiceEnabled = value;
                        break;
                    case 8:
                        settings.MovedVoiceEnabled = value;
                        break;
                    case 9:
                        settings.UserUpdatedEnabled = value;
                        break;
                    case 10:
                        settings.UserJoinedEnabled = value;
                        break;
                    case 11:
                        settings.UserLeftEnabled = value;
                        break;
                    case 12:
                        settings.UserBannedEnabled = value;
                        break;
                    case 13:
                        settings.UserUnbannedEnabled = value;
                        break;
                    case 14:
                        settings.InviteCreatedEnabled = value;
                        break;
                    case 15:
                        settings.InviteDeletedEnabled = value;
                        break;
                    case 16:
                        settings.RoleCreatedEnabled = value;
                        break;
                    case 17:
                        settings.RoleDeletedEnabled = value;
                        break;
                }

                Database.Settings.UpdateSettings();
                await RespondAsync("Setting successfully set", ephemeral: true);
            }
        }
    }
}
