using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Fergun.Interactive;

namespace Enclave_Bot.Core.Commands
{
    public class ModerationCommands : ModuleBase<SocketCommandContext>
    {
        public InteractiveService Interactive { get; set; }
    }
}
