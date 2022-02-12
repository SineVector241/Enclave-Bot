using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Enclave_Bot.Core.Commands
{
    public class Economy : ModuleBase<SocketCommandContext>
    {
        [Command("balance")]
        [Alias("bal")]
        [Summary("Sends your bank and wallet balance")]
        public async Task Test()
        {
            await Context.Channel.SendMessageAsync("Coming soon");
        }
    }
}
