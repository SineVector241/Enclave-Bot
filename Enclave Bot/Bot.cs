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
using Serilog;
using Fergun.Interactive;

namespace Enclave_Bot
{
    public class Bot
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public Bot()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.RollingFile(".\\Logs\\Enclave Bot-{Date}.txt")
                .CreateLogger();
            _services = BuildServiceProvider();
        }

        public async Task MainAsync()
        {
            await new CommandManager(_services).InitializeAsync();
            await new EventHandler(_services).InitAsync();

            _client.Log += Client_Log;

            if (string.IsNullOrWhiteSpace(Config.BotConfiguration.Token))
            {
                Log.Error("BotConfiguration Token is blank.");
                return;
            }

            await _client.LoginAsync(TokenType.Bot, Config.BotConfiguration.Token);
            await _client.StartAsync();
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
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();
        }
    }
}
