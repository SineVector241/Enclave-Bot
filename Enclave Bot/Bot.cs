using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Fergun.Interactive;
using Discord.Interactions;
using Enclave_Bot.Database;

namespace Enclave_Bot
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly InteractionService _interactions;

        public Bot()
        {
            Console.WriteLine($"[{DateTime.Now}]: [Bot] => Starting Bot...");
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
                UseInteractionSnowflakeDate = true,
                MessageCacheSize = 100,
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            });

            _interactions = new InteractionService(_client.Rest, new InteractionServiceConfig
            {
                LogLevel = LogSeverity.Debug
            });

            _serviceProvider = BuildServiceProvider();
        }

        public async Task MainAsync()
        {
            _serviceProvider.GetRequiredService<EventHandler>();
            await new InteractionManager(_serviceProvider).Initialize();

            _client.Log += ClientLog;
            if (string.IsNullOrWhiteSpace(Config.BotConfiguration.Token))
            {
                Console.WriteLine($"[{DateTime.Now}]: [ERROR] => An error occured in Bot.cs \nError Info:\nBOT CONFIGURATION TOKEN IS BLANK");
                return;
            }

            await _client.LoginAsync(TokenType.Bot, Config.BotConfiguration.Token);
            await _client.StartAsync();
            await Task.Delay(-1);
        }

        private static Task ClientLog(LogMessage msg)
        {
            Console.WriteLine($"[{DateTime.Now}]: [{msg.Source}] => {msg.Message}");
            return Task.CompletedTask;
        }

        private ServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<EventHandler>(x => new EventHandler(x))
                .AddSingleton<DatabaseContext>()
                .AddSingleton<InteractiveService>()
                .AddSingleton(_interactions)
                .BuildServiceProvider();
        }
    }
}
