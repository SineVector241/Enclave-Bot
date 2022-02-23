using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord.Interactions;
using Enclave_Bot.Core.Database;

namespace Enclave_Bot
{
    public class EventHandler
    {
        private DiscordSocketClient Client;
        private readonly InteractionService Interactions;
        private readonly IServiceProvider ServiceProvider;
        private Database db = new Database();

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

            //Discord Event Logging
            Client.UserJoined += UserJoined;
            Client.UserLeft += UserLeft;
            return Task.CompletedTask;
        }

        private Task UserLeft(SocketGuild guild, SocketUser user)
        {
            _ = Task.Run(async () =>
            {
                var gsettings = await db.GetGuildSettingsById(guild.Id);
                EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle($"{user.Username} has left!")
                .WithDescription(gsettings.LeaveMessage.Replace("[user]", user.Username).Replace("[guild]", guild.Name))
                .WithThumbnailUrl(user.GetAvatarUrl());
                await guild.GetTextChannel(gsettings.WelcomeChannel).SendMessageAsync(embed: embed.Build());
            });
            return Task.CompletedTask;
        }

        private Task UserJoined(SocketGuildUser user)
        {
            _ = Task.Run(async () =>
            {
                var gsettings = await db.GetGuildSettingsById(user.Guild.Id);
                EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle($"Welcome {user.Username} to {user.Guild.Name}!")
                .WithDescription(gsettings.WelcomeMessage.Replace("[user]",user.Username).Replace("[guild]", user.Guild.Name))
                .WithThumbnailUrl(user.GetAvatarUrl());
                await user.Guild.GetTextChannel(gsettings.WelcomeChannel).SendMessageAsync(user.Mention, embed: embed.Build());
            });
            return Task.CompletedTask;
        }


        //Important for the bots framework
        private async Task InteractionCreated(SocketInteraction interaction)
        {
            try
            {
                if (interaction.Channel is SocketGuildChannel)
                {
                    var guild = interaction.Channel as SocketGuildChannel;
                    if (guild != null)
                    {
                        if (!await db.GuildHasSettings(guild.Guild.Id))
                        {
                            await db.CreateGuildSettings(new GuildSettings()
                            {
                                GuildID = guild.Guild.Id,
                                WelcomeChannel = 0,
                                LoggingChannel = 0,
                                ApplicationChannel = 0,
                                ParchmentCategory = 0,
                                StaffApplicationChannel = 0,
                                UnverifiedRole = 0,
                                VerifiedRole = 0,
                                WelcomeMessage = "Welcome to [guild] **[user]**",
                                LeaveMessage = "We are sorry to see you go **[user]**"
                            });
                        }
                    }
                }
                if (interaction is SocketSlashCommand)
                {
                    var ctx = new SocketInteractionContext<SocketSlashCommand>(Client, (SocketSlashCommand)interaction);
                    await Interactions.ExecuteCommandAsync(ctx, ServiceProvider);
                }
                else if (interaction is SocketMessageComponent)
                {
                    var ctx = new SocketInteractionContext<SocketMessageComponent>(Client, (SocketMessageComponent)interaction);
                    await Interactions.ExecuteCommandAsync(ctx, ServiceProvider);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task ClientReady()
        {
            try
            {
                Console.WriteLine($"\u001b[97m[{DateTime.Now}]: [\u001b[92mREADY\u001b[97m] => {Client.CurrentUser.Username} is ready!");
                await Client.SetGameAsync("/help");
                await Client.SetStatusAsync(UserStatus.Online);
                await Interactions.RegisterCommandsToGuildAsync(749358542145716275);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\u001b[97m[{DateTime.Now}]: [\u001b[31mERROR\u001b[97m] => An error occured in EventHandler.cs \nError Info:\n{ex}");
            }
        }
    }
}
