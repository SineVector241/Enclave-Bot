using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord.Interactions;
using Enclave_Bot.Core.Database;
using Enclave_Bot.Core;

namespace Enclave_Bot
{
    public class EventHandler
    {
        private DiscordSocketClient Client;
        private readonly InteractionService Interactions;
        private readonly IServiceProvider ServiceProvider;

        public EventHandler(IServiceProvider Services)
        {
            ServiceProvider = Services;
            Client = Services.GetRequiredService<DiscordSocketClient>();
            Interactions = Services.GetRequiredService<InteractionService>();
        }

        public Task Initialize()
        {
            //Create Event Listeners Here
            Client.Ready += ClientReady;
            Client.InteractionCreated += InteractionCreated;
            Interactions.InteractionExecuted += InteractionExecuted;

            //Discord Event Logging
            Client.MessageDeleted += MessageDeleted;
            Client.MessageUpdated += MessageUpdated;

            Client.ChannelCreated += ChannelCreated;
            Client.ChannelUpdated += ChannelUpdated;
            Client.ChannelDestroyed += ChannelDeleted;

            Client.UserVoiceStateUpdated += UserVoiceStateUpdated;

            Client.GuildMemberUpdated += GuildMemberUpdated;
            Client.UserJoined += UserJoined;
            Client.UserLeft += UserLeft;
            Client.UserBanned += UserBanned;
            Client.UserUnbanned += UserUnbanned;

            Client.UserJoined += WelcomeEvent;
            Client.UserLeft += GoodbyeEvent;

            Client.InviteCreated += InviteCreated;
            Client.InviteDeleted += InviteDeleted;

            Client.RoleCreated += RoleCreated;
            Client.RoleDeleted += RoleDeleted;

            Client.MessageReceived += MessageReceived;

            return Task.CompletedTask;
        }

        //Role Logging Events
        private Task RoleDeleted(SocketRole role)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;

                //Do Some Checks
                if (!settings.LoggingEnabled || !settings.RoleDeletedEnabled) return;

                var logChannel = role.Guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                var embed = new EmbedBuilder()
                   .WithTitle("Role Deleted")
                   .AddField("Role", role.Name)
                   .AddField("Color", role.Color.ToString())
                   .WithColor(Color.Red);

                await logChannel.SendMessageAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        private Task RoleCreated(SocketRole role)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;

                //Do Some Checks
                if (!settings.LoggingEnabled || !settings.RoleCreatedEnabled) return;

                var logChannel = role.Guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                var embed = new EmbedBuilder()
                   .WithTitle("Role Created")
                   .AddField("Role", role.Mention)
                   .AddField("Color", role.Color.ToString())
                   .WithColor(Color.Green);

                await logChannel.SendMessageAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        //Invite Logging Events
        private Task InviteCreated(SocketInvite invite)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;

                //Do Some Checks
                if (!settings.LoggingEnabled || !settings.InviteCreatedEnabled) return;

                var logChannel = invite.Guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                var embed = new EmbedBuilder()
                   .WithTitle("Invite Created")
                   .AddField("Invite Code", invite.Code)
                   .AddField("Invite Creator", invite.Inviter.Mention)
                   .WithColor(Color.Green);

                await logChannel.SendMessageAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        private Task InviteDeleted(SocketGuildChannel guildChannel, string code)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;

                //Do Some Checks
                if (!settings.LoggingEnabled || !settings.InviteDeletedEnabled) return;

                var logChannel = guildChannel.Guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                var embed = new EmbedBuilder()
                   .WithTitle("Invite Deleted")
                   .AddField("Invite Code", code)
                   .WithColor(Color.Red);

                await logChannel.SendMessageAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        //Welcome/Goodbye Events
        private Task WelcomeEvent(SocketGuildUser user)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.GreetingSettings;
                var welcomeChannel = user.Guild.GetTextChannel(settings.GreetingChannel);

                if (settings.JoinMessageTitle == null || welcomeChannel == null || !settings.SendJoinMessage) return;

                var joinRole = user.Guild.GetRole(settings.JoinRole);
                var embed = new EmbedBuilder()
                    .WithTitle(settings.JoinMessageTitle.Replace("[user]", user.Username).Replace("[guild]", user.Guild.Name))
                    .WithColor(Color.Red);

                if (!string.IsNullOrWhiteSpace(settings.JoinMessage))
                    embed.WithDescription(settings.JoinMessage.Replace("[user]", user.Username).Replace("[guild]", user.Guild.Name));

                if (settings.ShowProfileInThumbnail)
                    embed.WithThumbnailUrl(user.GetAvatarUrl());

                if (settings.ShowProfileInPicture)
                    embed.WithImageUrl(user.GetAvatarUrl());

                if(settings.AddJoinRole && joinRole != null)
                {
                    await user.AddRoleAsync(joinRole);
                }

                if (settings.MentionUser)
                {
                    await welcomeChannel.SendMessageAsync(user.Mention, embed: embed.Build());
                    return;
                }

                await welcomeChannel.SendMessageAsync(user.Mention, embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        private Task GoodbyeEvent(SocketGuild guild, SocketUser user)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.GreetingSettings;
                var welcomeChannel = guild.GetTextChannel(settings.GreetingChannel);

                if (settings.LeaveMessageTitle == null || welcomeChannel == null || !settings.SendLeaveMessage) return;

                var embed = new EmbedBuilder()
                    .WithTitle(settings.LeaveMessageTitle.Replace("[user]", user.Username).Replace("[guild]", guild.Name))
                    .WithColor(Color.Red);

                if (!string.IsNullOrWhiteSpace(settings.LeaveMessage))
                    embed.WithDescription(settings.LeaveMessage.Replace("[user]", user.Username).Replace("[guild]", guild.Name));

                if (settings.ShowProfileInThumbnail)
                    embed.WithThumbnailUrl(user.GetAvatarUrl());

                if (settings.ShowProfileInPicture)
                    embed.WithImageUrl(user.GetAvatarUrl());

                if (settings.MentionUser)
                {
                    await welcomeChannel.SendMessageAsync(user.Mention, embed: embed.Build());
                    return;
                }

                await welcomeChannel.SendMessageAsync(user.Mention, embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        //User Logging Events
        private Task UserUnbanned(SocketUser user, SocketGuild guild)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;

                //Do Some Checks
                if (!settings.LoggingEnabled || !settings.UserUnbannedEnabled) return;

                var logChannel = guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                var embed = new EmbedBuilder()
                   .WithTitle("User Unbanned")
                   .AddField("User", user.Username)
                   .WithColor(Color.Green);

                await logChannel.SendMessageAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        private Task UserBanned(SocketUser user, SocketGuild guild)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;

                //Do Some Checks
                if (!settings.LoggingEnabled || !settings.UserBannedEnabled) return;

                var logChannel = guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                var embed = new EmbedBuilder()
                   .WithTitle("User Banned")
                   .AddField("User", user.Username)
                   .WithColor(Color.Red);

                await logChannel.SendMessageAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        private Task UserLeft(SocketGuild guild, SocketUser user)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;

                //Do Some Checks
                if (!settings.LoggingEnabled || !settings.UserLeftEnabled) return;

                var logChannel = guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                var embed = new EmbedBuilder()
                   .WithTitle("User Left")
                   .AddField("User", user.Username)
                   .WithColor(Color.Red);

                await logChannel.SendMessageAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        private Task UserJoined(SocketGuildUser user)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;

                //Do Some Checks
                if (!settings.LoggingEnabled || !settings.UserJoinedEnabled) return;

                var logChannel = user.Guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                var embed = new EmbedBuilder()
                   .WithTitle("User Joined")
                   .AddField("User", user.Mention)
                   .WithColor(Color.Green);

                await logChannel.SendMessageAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        private Task GuildMemberUpdated(Cacheable<SocketGuildUser, ulong> guildUserBefore, SocketGuildUser guildUserAfter)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var settings = Settings.Current.LoggingSettings;

                    //Do Some Checks
                    if (!settings.LoggingEnabled || !settings.UserUpdatedEnabled) return;

                    var logChannel = guildUserAfter.Guild.GetTextChannel(settings.LoggingChannel);
                    if (logChannel == null) return;

                    var roleList = "";
                    for (int i = 0; i < guildUserAfter.Roles.Count; i++)
                    {
                        var role = guildUserAfter.Roles.ElementAt(i);
                        if (!guildUserBefore.Value.Roles.Contains(role))
                        {
                            roleList += $"**+:** {role.Mention}\n";
                        }
                    }
                    for (int i = 0; i < guildUserBefore.Value.Roles.Count; i++)
                    {
                        var role = guildUserBefore.Value.Roles.ElementAt(i);
                        if (!guildUserAfter.Roles.Contains(role))
                        {
                            roleList += $"**-:** {role.Mention}\n";
                        }
                    }

                    var embed = new EmbedBuilder()
                        .WithTitle("User Updated")
                        .AddField("Username", guildUserAfter.Username)
                        .AddField("Nickname", guildUserBefore.Value.Nickname != guildUserAfter.Nickname ? $"{guildUserBefore.Value.Nickname} => {guildUserAfter.Nickname}" : guildUserAfter.DisplayName)
                        .AddField("Roles", roleList)
                        .WithColor(Color.Orange);

                    await logChannel.SendMessageAsync(embed: embed.Build());
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });

            return Task.CompletedTask;
        }

        //Voice Logging Events
        private Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState stateBefore, SocketVoiceState stateAfter)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;
                var guildUser = user as SocketGuildUser;

                //Do Some Checks
                if (guildUser == null || !settings.LoggingEnabled) return;

                var logChannel = guildUser.Guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                if (stateBefore.IsMuted != stateAfter.IsMuted ||
                stateBefore.IsDeafened != stateAfter.IsDeafened ||
                stateBefore.IsSuppressed != stateAfter.IsSuppressed ||
                stateBefore.IsStreaming != stateAfter.IsStreaming ||
                stateBefore.IsVideoing != stateAfter.IsVideoing ||
                stateBefore.IsSelfDeafened != stateAfter.IsSelfDeafened ||
                stateBefore.IsSelfMuted != stateAfter.IsSelfMuted) return;

                if (settings.JoinedVoiceEnabled && stateBefore.VoiceChannel == null && stateAfter.VoiceChannel != null)
                {
                    var embed = new EmbedBuilder()
                        .WithTitle("Joined Voice")
                        .AddField("User", guildUser.Username)
                        .AddField("Channel", stateAfter.VoiceChannel.Mention)
                        .WithColor(Color.Green);

                    await logChannel.SendMessageAsync(embed: embed.Build());
                }
                else if (settings.MovedVoiceEnabled && stateBefore.VoiceChannel != null && stateAfter.VoiceChannel != null)
                {
                    var embed = new EmbedBuilder()
                        .WithTitle("Moved Voice")
                        .AddField("User", guildUser.Username)
                        .AddField("From", stateBefore.VoiceChannel.Mention)
                        .AddField("To", stateAfter.VoiceChannel.Mention)
                        .WithColor(Color.Orange);

                    await logChannel.SendMessageAsync(embed: embed.Build());
                }
                else if (settings.MovedVoiceEnabled && stateBefore.VoiceChannel != null && stateAfter.VoiceChannel == null)
                {
                    var embed = new EmbedBuilder()
                        .WithTitle("Disconnect Voice")
                        .AddField("User", guildUser.Username)
                        .AddField("Channel", stateBefore.VoiceChannel.Mention)
                        .WithColor(Color.Red);

                    await logChannel.SendMessageAsync(embed: embed.Build());
                }
            });

            return Task.CompletedTask;
        }

        //Channel Logging Events
        private Task ChannelDeleted(SocketChannel channel)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;
                var chn = channel as SocketGuildChannel;

                //Do Some Checks
                if (chn == null || !settings.LoggingEnabled || !settings.ChannelDeletedEnabled) return;

                var logChannel = chn.Guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                //Start Building The Log Message
                var embed = new EmbedBuilder()
                    .WithTitle("Channel Deleted")
                    .AddField("Channel", chn.Name)
                    .AddField("Type", chn.GetType().Name)
                    .WithColor(Color.Red);

                await logChannel.SendMessageAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        private Task ChannelUpdated(SocketChannel chnBefore, SocketChannel chnAfter)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;
                var chn = chnBefore as SocketGuildChannel;
                var chnA = chnAfter as SocketGuildChannel;

                //Do Some Checks
                if (chn == null || chnA == null || !settings.LoggingEnabled || !settings.ChannelUpdatedEnabled) return;

                var logChannel = chn.Guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                //Start Building The Log Message
                var embed = new EmbedBuilder()
                    .WithTitle("Channel Updated")
                    .AddField("Channel Before", chn.Name)
                    .AddField("Channel After", chnA.Name)
                    .AddField("Type", chn.GetType().Name)
                    .WithColor(Color.Orange);

                await logChannel.SendMessageAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        private Task ChannelCreated(SocketChannel channel)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;
                var chn = channel as SocketGuildChannel;

                //Do Some Checks
                if (chn == null || !settings.LoggingEnabled || !settings.ChannelCreatedEnabled) return;

                var logChannel = chn.Guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                //Start Building The Log Message
                var embed = new EmbedBuilder()
                    .WithTitle("Channel Created")
                    .AddField("Channel", chn.Name)
                    .AddField("Type", chn.GetType().Name)
                    .WithColor(Color.Green);

                await logChannel.SendMessageAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        //Message Logging Events
        private Task MessageUpdated(Cacheable<IMessage, ulong> msgBefore, SocketMessage msgAfter, ISocketMessageChannel channel)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;
                var chn = channel as SocketTextChannel;

                //Do Some Checks
                if (chn == null || !settings.LoggingEnabled || !settings.MessageEditedEnabled || !msgBefore.HasValue
                 || msgBefore.Value.CleanContent == msgAfter.CleanContent) return;

                var logChannel = chn.Guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                //Start Building The Log Message
                var embed = new EmbedBuilder()
                    .WithTitle("Message Edited")
                    .AddField("Author", msgAfter.Author.Username)
                    .AddField("Channel", chn.Mention)
                    .AddField("Before", msgBefore.Value.CleanContent != null ? msgBefore.Value.CleanContent : "`No Message`")
                    .AddField("After", msgAfter.CleanContent != null ? msgAfter.CleanContent : "`No Message`")
                    .WithColor(Color.Orange);

                if (msgAfter.Attachments.Count > 0)
                    embed.AddField("Attachments", msgAfter.Attachments.Count);

                await logChannel.SendMessageAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        private Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        {
            _ = Task.Run(async () =>
            {
                var settings = Settings.Current.LoggingSettings;
                var chn = channel.Value as SocketTextChannel;

                //Do Some Checks
                if (chn == null || !settings.LoggingEnabled || !settings.MessageDeletedEnabled || !message.HasValue) return;

                var logChannel = chn.Guild.GetTextChannel(settings.LoggingChannel);
                if (logChannel == null) return;

                //Start Building The Log Message
                var embed = new EmbedBuilder()
                    .WithTitle("Message Deleted")
                    .AddField("Author", message.Value.Author.Username)
                    .AddField("Channel", chn.Mention)
                    .AddField("Message Content", message.Value.CleanContent != null ? message.Value.CleanContent : "`No Message`")
                    .WithColor(Color.Red);

                if (message.Value.Attachments.Count > 0)
                {
                    var links = "";
                    for (int i = 0; i < message.Value.Attachments.Count; i++)
                    {
                        links += message.Value.Attachments.ElementAt(i).ProxyUrl + "\n";
                    }
                    embed.AddField("Attachments", links);
                }

                await logChannel.SendMessageAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }

        private Task MessageReceived(SocketMessage msg)
        {
            try
            {
                var UserId = msg.Author.Id;
                if (msg.Channel is SocketDMChannel || msg.Author.IsBot) return Task.CompletedTask;

                var user = Users.GetUserById(UserId);
                user.LastActiveUnix = $"<t:{Utils.ToUnixTimestamp(DateTime.UtcNow)}:R>";
                Users.UpdateUser(user);
                return Task.CompletedTask;
            }
            catch
            {
                return Task.CompletedTask;
            }
        }

        private async Task InteractionExecuted(ICommandInfo cmdInfo, IInteractionContext ctx, IResult result)
        {
            if (!result.IsSuccess)
            {
                var errorEmbed = new EmbedBuilder()
                    .WithTitle("Error!")
                    .WithDescription($"**Error Message:** {result.ErrorReason}")
                    .WithAuthor(ctx.User)
                    .WithColor(Color.Red).Build();

                if (ctx.Interaction.HasResponded)
                    await ctx.Interaction.FollowupAsync(embed: errorEmbed);
                else
                    await ctx.Interaction.RespondAsync(embed: errorEmbed);
            }
        }

        //Important for the bots framework
        private async Task InteractionCreated(SocketInteraction interaction)
        {
            try
            {
                if (interaction is SocketSlashCommand)
                {
                    var ctx = new SocketInteractionContext<SocketSlashCommand>(Client, (SocketSlashCommand)interaction);
                    var result = await Interactions.ExecuteCommandAsync(ctx, ServiceProvider);
                }
                else if (interaction is SocketMessageComponent)
                {
                    var ctx = new SocketInteractionContext<SocketMessageComponent>(Client, (SocketMessageComponent)interaction);
                    var result = await Interactions.ExecuteCommandAsync(ctx, ServiceProvider);
                }

                var user = Users.GetUserById(interaction.User.Id);
                user.LastActiveUnix = $"<t:{Utils.ToUnixTimestamp(DateTime.UtcNow)}:R>";
                Users.UpdateUser(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}]: [ERROR] => An error occured in EventHandler.cs \nError Info:\n{ex}");
            }
        }

        //When the client is ready.
        private async Task ClientReady()
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now}]: [READY] => {Client.CurrentUser.Username} is ready!");
                await Client.SetGameAsync("/help");
                await Client.SetStatusAsync(UserStatus.Online);
                await Interactions.RegisterCommandsToGuildAsync(749358542145716275);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}]: [ERROR] => An error occured in EventHandler.cs \nError Info:\n{ex}");
            }
        }
    }
}
