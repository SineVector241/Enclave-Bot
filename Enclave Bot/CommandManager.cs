using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Enclave_Bot
{
    public class CommandManager
    {
        private readonly DiscordSocketClient Client;
        private readonly CommandService Commands;
        private readonly IServiceProvider _Services;

        public CommandManager(IServiceProvider Services)
        {
            Client = Services.GetRequiredService<DiscordSocketClient>();
            Commands = Services.GetRequiredService<CommandService>();
            _Services = Services;
        }

        public async Task InitializeAsync()
        {
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), _Services);
            foreach (var cmd in Commands.Commands)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{DateTime.Now} => [COMMANDS]: {cmd.Name} loaded");
            }
            Commands.Log += Command_Log;
        }

        private Task Command_Log(LogMessage Command)
        {
            Console.WriteLine(Command.Message);
            return Task.CompletedTask;
        }
    }
}
