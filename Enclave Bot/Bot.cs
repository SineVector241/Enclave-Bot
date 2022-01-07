using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.Linq;

namespace Enclave_Bot
{
    public class Bot
    {
        private DiscordSocketClient Client;
        private CommandService Commands;

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
        }

        public async Task MainAsync()
        {
            CommandManager cmdManager = new CommandManager(Client, Commands);
            await cmdManager.InitializeAsync();

            Client.Ready += Client_Ready;
            Client.Log += Client_Log;
            if (Config.bot.Token == "" || Config.bot.Token == null) return;

            await Client.LoginAsync(TokenType.Bot, Config.bot.Token);
            await Client.StartAsync();
            await Task.Delay(-1);
        }

        private Task Client_Log(LogMessage Message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{DateTime.Now} At {Message.Source}: {Message.Message}");
            Console.ForegroundColor = ConsoleColor.White;
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
    }
}
