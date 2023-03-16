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

                foreach (ModuleInfo module in Interactions.Modules)
                {
                    Console.WriteLine($"[{DateTime.Now}]: [MODULES] => {module.Name} Initialized");
                    Interactions.Log += InteractionServiceLog;
                }
                foreach (SlashCommandInfo cmd in Interactions.SlashCommands)
                {
                    Console.WriteLine($"[{DateTime.Now}]: [SLASHCOMMAND] => {cmd.Name} Loaded");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}]: [ERROR] => An error occured in InteractioneManager.cs \nError Info:\n{ex}");
            }
        }

        private Task InteractionServiceLog(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
            return Task.CompletedTask;
        }
    }
}
