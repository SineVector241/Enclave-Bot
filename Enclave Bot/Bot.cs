using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Fergun.Interactive;
using Discord.Interactions;

namespace Enclave_Bot
{
    public class Bot
    {
        private DiscordSocketClient Client;
        private IServiceProvider ServiceProvider;
        private InteractionService Interactions;

        public Bot()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
                UseInteractionSnowflakeDate = false,
                MessageCacheSize = 100,
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            });

            Interactions = new InteractionService(Client.Rest, new InteractionServiceConfig
            {
                LogLevel = LogSeverity.Debug
            });

            ServiceProvider = BuildServiceProvider();
        }

        public async Task MainAsync()
        {
            await new EventHandler(ServiceProvider).Initialize();
            await new InteractionManager(ServiceProvider).Initialize();

            Client.Log += ClientLog;
            if (string.IsNullOrWhiteSpace(Config.BotConfiguration.Token))
            {
                Console.WriteLine($"[{DateTime.Now}]: [ERROR] => An error occured in Bot.cs \nError Info:\nBOT CONFIGURATION TOKEN IS BLANK");
                return;
            }

            await Client.LoginAsync(TokenType.Bot, Config.BotConfiguration.Token);
            await Client.StartAsync();
            await Task.Delay(-1);
        }

        private Task ClientLog(LogMessage msg)
        {
            Console.WriteLine($"[{DateTime.Now}]: [{msg.Source}] => {msg.Message}");
            return Task.CompletedTask;
        }

        private ServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton<InteractiveService>()
                .AddSingleton(Interactions)
                .BuildServiceProvider();
        }
    }
}
