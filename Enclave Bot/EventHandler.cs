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
        private DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly InteractionService _interactionService;
        private Database db = new Database();

        public EventHandler(IServiceProvider Services)
        {
            _services = Services;
            _client = Services.GetRequiredService<DiscordSocketClient>();
            _commands = Services.GetRequiredService<CommandService>();
            _interactionService = Services.GetRequiredService<InteractionService>();
        }

        public Task InitAsync()
        {
            //Create Event Listeners Here
            _client.MessageReceived += Message_Event;
            _client.Ready += Client_Ready;
            _client.InteractionCreated += _client_InteractionCreated;
            _interactionService.ComponentCommandExecuted += _interactionService_ComponentCommandExecuted;

            //Discord Event Logging
            _client.UserJoined += UserJoined;
            _client.UserLeft += UserLeft;
            _client.MessageDeleted += MessageDeleted;
            _client.MessageUpdated += MessageUpdated;
            _client.MessagesBulkDeleted += MessagesBulkDeleted;
            return Task.CompletedTask;
        }

        //Important for the bots framework
        private Task _interactionService_ComponentCommandExecuted(ComponentCommandInfo arg1, IInteractionContext arg2, Discord.Interactions.IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }
            return Task.CompletedTask;
        }

        private async Task _client_InteractionCreated(SocketInteraction arg)
        {
            var ctx = new SocketInteractionContext<SocketMessageComponent>(_client, (SocketMessageComponent)arg);
            await _interactionService.ExecuteCommandAsync(ctx, _services);
        }

        private async Task Client_Ready()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{DateTime.Now} => [READY EVENT]: {_client.CurrentUser.Username} is ready");
            Console.ForegroundColor = ConsoleColor.White;
            await _client.SetGameAsync($"{Config.BotConfiguration.Prefix}help");
            await _client.SetStatusAsync(UserStatus.Online);
        }

        private async Task Message_Event(SocketMessage messageParameter)
        {
            try
            {
                SocketUserMessage message = messageParameter as SocketUserMessage;
                SocketCommandContext context = new SocketCommandContext(_client, message);

                if (context.Message == null || context.Message.Content == "")
                {
                    return;
                }

                if (context.User.IsBot)
                {
                    return;
                }

                if(context.Channel is SocketTextChannel && !await db.GuildHasSettings(context.Guild.Id))
                {
                    await db.CreateGuildSettings(new GuildSettings { 
                        GuildID = context.Guild.Id,
                        LoggingChannel = 0,
                        WelcomeChannel = 0,
                        ApplicationChannel = 0,
                        StaffApplicationChannel = 0,
                        ParchmentCategory = 0,
                        VerifiedRole = 0,
                        UnverifiedRole = 0
                    });
                }

                int argPos = 0;

                if (!(message.HasStringPrefix(Config.BotConfiguration.Prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                {
                    return;
                }

                Discord.Commands.IResult result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Log.Debug($"{DateTime.Now} at Command: {_commands.Search(context, argPos).Commands[0].Command.Name} in {_commands.Search(context, argPos).Commands[0].Command.Module.Name} {result.ErrorReason}");
                    Console.ForegroundColor = ConsoleColor.White;
                    EmbedBuilder Embed = new EmbedBuilder();

                    if (result.ErrorReason == "The input text has too few paramaters")
                    {
                        Embed.WithTitle("Error");
                        Embed.WithDescription("This command requires a missing paramater. Please check help command to see what it needs");
                    }
                    else
                    {
                        Embed.WithTitle("Error");
                        Embed.WithDescription(result.ErrorReason);
                    }
                    await context.Channel.SendMessageAsync("", embed: Embed.Build());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log.Error(string.Format("[0} - {1}", e.InnerException?.Message ?? e.Message, e.StackTrace));
            }
        }

        //Logging events
        private async Task UserJoined(SocketGuildUser user)
        {
            var gsettings = await db.GetGuildSettingsById(user.Guild.Id);
            EmbedBuilder embed = new EmbedBuilder()
            .WithColor(Color.Green)
            .WithTitle($"Welcome {user.Username} to {user.Guild.Name}!")
            .WithDescription("Make sure to read <#757596592583868416> then you can submit an app. Be sure to get your <@&934849915564224512> role in <#936802164238598184>")
            .WithThumbnailUrl(user.GetAvatarUrl());
            await user.Guild.GetTextChannel(gsettings.WelcomeChannel).SendMessageAsync(user.Mention,embed: embed.Build());
        }

        private async Task UserLeft(SocketGuild guild, SocketUser user)
        {
            var gsettings = await db.GetGuildSettingsById(guild.Id);
            EmbedBuilder embed = new EmbedBuilder()
            .WithColor(Color.Green)
            .WithTitle($"Sorry to see you go {user.Username}")
            .WithDescription("Be safe on your travels!")
            .WithThumbnailUrl(user.GetAvatarUrl());
            await guild.GetTextChannel(gsettings.WelcomeChannel).SendMessageAsync(embed: embed.Build());
        }

        private async Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> arg2)
        {
            try
            {
                var channel = await arg2.GetOrDownloadAsync() as SocketGuildChannel;
                var loggingchannel = channel.Guild.GetTextChannel(db.GetGuildSettingsById(channel.Guild.Id).GetAwaiter().GetResult().LoggingChannel);
                EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithTitle("Message Deleted")
                .WithThumbnailUrl(message.Value.Author.GetAvatarUrl())
                .AddField("Author", message.Value.Author.Mention)
                .AddField("Channel", $"<#{channel.Id}>")
                .AddField("Message Content", message.Value.Content);
                await loggingchannel.SendMessageAsync(embed:embed.Build());
            }
            catch (Exception ex)
            {

            }
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage after, ISocketMessageChannel arg3)
        {
            try
            {
                var channel = arg3 as SocketGuildChannel;
                var before = await arg1.GetOrDownloadAsync();
                if (before.Content == after.Content)
                    return;
                var loggingchannel = channel.Guild.GetTextChannel(db.GetGuildSettingsById(channel.Guild.Id).GetAwaiter().GetResult().LoggingChannel);
                EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.LightOrange)
                .WithTitle("Message Edited")
                .WithThumbnailUrl(after.Author.GetAvatarUrl())
                .AddField("Author", after.Author.Mention)
                .AddField("Channel", $"<#{channel.Id}>")
                .AddField("Before", before.Content)
                .AddField("After", after.Content);
                await loggingchannel.SendMessageAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {

            }
        }

        private async Task MessagesBulkDeleted(IReadOnlyCollection<Cacheable<IMessage, ulong>> arg1, Cacheable<IMessageChannel, ulong> arg2)
        {
            var channel = await arg2.GetOrDownloadAsync() as SocketGuildChannel;
            var loggingchannel = channel.Guild.GetTextChannel(db.GetGuildSettingsById(channel.Guild.Id).GetAwaiter().GetResult().LoggingChannel);
            var embed = new EmbedBuilder()
                .WithTitle("Message Bulk Deletion")
                .AddField("Channel", $"<#{channel.Id}>")
                .WithColor(Color.DarkRed);
            await loggingchannel.SendMessageAsync(embed: embed.Build());
        }
    }
}
