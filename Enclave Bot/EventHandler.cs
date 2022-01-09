using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Enclave_Bot
{
    public class EventHandler
    {
        private DiscordSocketClient Client;
        private readonly CommandService Commands;
        private readonly IServiceProvider _Services;

        public EventHandler(IServiceProvider Services)
        {
            _Services = Services;
            Client = Services.GetRequiredService<DiscordSocketClient>();
            Commands = Services.GetRequiredService<CommandService>();
        }

        public Task InitAsync()
        {
            Client.MessageReceived += Message_Event;
            Client.Ready += Client_Ready;
            return Task.CompletedTask;
        }

        private async Task Client_Ready()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{DateTime.Now}: {Client.CurrentUser.Username} is ready");
            Console.ForegroundColor = ConsoleColor.White;
            await Client.SetGameAsync($"{Config.bot.Prefix}help");
            await Client.SetStatusAsync(UserStatus.Online);
        }

        private async Task Message_Event(SocketMessage MessageParamater)
        {
            var Message = MessageParamater as SocketUserMessage;
            var Context = new SocketCommandContext(Client, Message);

            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;

            int ArgPos = 0;
            if (!(Message.HasStringPrefix(Config.bot.Prefix, ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))) return;
            var Result = await Commands.ExecuteAsync(Context, ArgPos, _Services);

            if (!Result.IsSuccess && Result.Error != CommandError.UnknownCommand)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{DateTime.Now} at Command: {Commands.Search(Context, ArgPos).Commands[0].Command.Name} in {Commands.Search(Context, ArgPos).Commands[0].Command.Module.Name} {Result.ErrorReason}");
                Console.ForegroundColor = ConsoleColor.White;
                var Embed = new EmbedBuilder();
                if (Result.ErrorReason == "The input text has too few paramaters")
                {
                    Embed.WithTitle("Error");
                    Embed.WithDescription("This command requires a missing paramater. Please check help command to see what it needs");
                }
                else
                {
                    Embed.WithTitle("Error");
                    Embed.WithDescription(Result.ErrorReason);
                }
                await Context.Channel.SendMessageAsync("", embed: Embed.Build());
            }
        }
    }
}
