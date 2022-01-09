using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Enclave_Bot
{
    public class Bot
    {
        private DiscordSocketClient Client;
        private CommandService Commands;
        private IServiceProvider Services;

        public Bot()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });
            Services = BuildServiceProvider();
        }

        public async Task MainAsync()
        {
            await new CommandManager(Services).InitializeAsync();
            await new EventHandler(Services).InitAsync();

            Client.Log += Client_Log;
            if (string.IsNullOrWhiteSpace(Config.bot.Token)) return;

            await Client.LoginAsync(TokenType.Bot, Config.bot.Token);
            await Client.StartAsync();
            await Task.Delay(-1);
        }

        private Task Client_Log(LogMessage Message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{DateTime.Now} => [{Message.Source}]: {Message.Message}");
            Console.ForegroundColor = ConsoleColor.White;
            return Task.CompletedTask;
        }

        private ServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();
        }
    }
}
