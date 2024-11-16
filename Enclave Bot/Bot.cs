using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Fergun.Interactive;
using Discord.Interactions;
using Enclave_Bot.Database;
using Enclave_Bot.Core;

namespace Enclave_Bot
{
    public class Bot
    {
        //Display/Message Limits!
        public const int TitleLengthLimit = 45;
        public static readonly Color PrimaryColor = Color.DarkOrange;
        public static readonly Color SecondaryColor = Color.LightOrange;

        private readonly DiscordSocketClient Client;
        private readonly IServiceProvider ServiceProvider;
        private readonly InteractionService Interactions;

        public Bot()
        {
            Console.WriteLine($"[{DateTime.Now}]: [Bot] => Starting Bot...");
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
            new EventHandler(ServiceProvider);
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
                .AddSingleton<Utils>()
                .AddSingleton<InteractiveService>()
                .AddSingleton(Interactions)
                .AddNpgsql<DatabaseContext>(Config.BotConfiguration.SqlConnection)
                .BuildServiceProvider();
        }
    }
}
