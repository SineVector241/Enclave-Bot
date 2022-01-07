using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Enclave_Bot.Core.Commands
{
    public class Miscellaneous : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Alias("latency")]
        [Summary("Displays the bots current latency")]
        public async Task PingCommand()
        {
            await Context.Channel.SendMessageAsync($"Pong: {Context.Client.Latency} ms");
        }

        [Command("Test")]
        [Alias("testy")]
        [Summary("Does testing stuff")]
        public async Task Test()
        {
            
        }
    }
}
