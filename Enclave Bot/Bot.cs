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
                GatewayIntents = GatewayIntents.All
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
            System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            if (string.IsNullOrWhiteSpace(Config.BotConfiguration.Token))
            {
                Console.WriteLine("\u001b[41mBOT CONFIGURATION TOKEN IS BLANK\u001b[40m");
                return;
            }

            await Client.LoginAsync(TokenType.Bot, Config.BotConfiguration.Token);
            await Client.StartAsync();
            await Task.Delay(-1);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"Error: Sender: {sender}\nUnhandled Event: e");
        }

        private Task ClientLog(LogMessage msg)
        {
            Console.WriteLine($"\u001b[97m[{DateTime.Now}]: [\u001b[93m{msg.Source}\u001b[97m] => {msg.Message}");
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
