using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Serilog;

namespace Enclave_Bot
{
    public class InteractionManager
    {
        private readonly IServiceProvider _service;
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactionService;
        
        public InteractionManager(IServiceProvider Services)
        {
            _service = Services;
            _client = Services.GetRequiredService<DiscordSocketClient>();
            _interactionService = Services.GetRequiredService<InteractionService>();
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _service);

                foreach (ModuleInfo interaction in _interactionService.Modules)
                {
                    Log.Debug($"{DateTime.Now} => [MODULES]: {interaction.Name} initialized");
                }
                _interactionService.Log += _interactionService_Log;
            }
            catch (Exception e)
            {
                Log.Error(string.Format("{0} - {1}", e.InnerException?.Message ?? e.Message, e.StackTrace));
            }
        }

        private Task _interactionService_Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
            return Task.CompletedTask;
        }
    }
}
