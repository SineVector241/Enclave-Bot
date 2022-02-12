using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Fergun.Interactive;
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
                                VerifiedRole = 0
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
