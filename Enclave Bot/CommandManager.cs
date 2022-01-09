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
using Serilog;

namespace Enclave_Bot
{
    public class CommandManager
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public CommandManager(IServiceProvider Services)
        {
            _client = Services.GetRequiredService<DiscordSocketClient>();
            _commands = Services.GetRequiredService<CommandService>();
            _services = Services;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

                foreach (CommandInfo cmd in _commands.Commands)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Log.Debug($"{DateTime.Now} => [COMMANDS]: {cmd.Name} loaded");
                }
                _commands.Log += Command_Log;
            }
            catch (Exception e)
            {
                Log.Error(string.Format("{0} - {1}", e.InnerException?.Message ?? e.Message, e.StackTrace));
            }
        }

        private Task Command_Log(LogMessage command)
        {
            Log.Debug(command.Message);
            return Task.CompletedTask;
        }
    }
}
