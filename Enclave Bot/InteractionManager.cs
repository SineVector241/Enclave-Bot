using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Enclave_Bot
{
    public class InteractionManager
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly InteractionService Interactions;
        private readonly DiscordSocketClient Client;

        public InteractionManager(IServiceProvider Services)
        {
            ServiceProvider = Services;
            Interactions = ServiceProvider.GetRequiredService<InteractionService>();
            Client = ServiceProvider.GetRequiredService<DiscordSocketClient>();
        }

        public async Task Initialize()
        {
            try
            {
                await Interactions.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider);
                Interactions.Log += InteractionServiceLog;

                foreach (ModuleInfo module in Interactions.Modules)
                {
                    Console.WriteLine($"[{DateTime.Now}]: [Modules] => {module.Name} Initialized");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}]: [Error] => An error occured in InteractionManager.cs \nError Info:\n{ex}");
            }
        }

        private Task InteractionServiceLog(LogMessage msg)
        {
            Console.WriteLine($"[{DateTime.Now}]: [Interaction] => {msg.Message}");
            return Task.CompletedTask;
        }
    }
}
