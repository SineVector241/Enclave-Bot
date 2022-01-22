using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Enclave_Bot.Core.Database;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Fergun.Interactive;
using Discord.Interactions;

namespace Enclave_Bot
{
    public class EventHandler
    {
        private DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly InteractionService _interactionService;

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
            return Task.CompletedTask;
        }

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
                
                Database db = new Database();
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
                if(!db.HasAccount(context.User.Id))
                {
                    db.CreateAccount(new User
                    {
                        DiscordID = context.User.Id,
                        Wallet = 0,
                        Bank = 200,
                        Level = 0,
                        XP = 0
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
                Log.Error(string.Format("[0} - {1}", e.InnerException?.Message ?? e.Message, e.StackTrace));
            }
        }
    }
}
