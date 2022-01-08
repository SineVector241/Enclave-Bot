using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace Enclave_Bot
{
    public class CommandManager
    {
        private readonly DiscordSocketClient _Client;
        private readonly CommandService _Commands;

        public CommandManager(DiscordSocketClient Client, CommandService Commands)
        {
            _Client = Client;
            _Commands = Commands;
        }

        public async Task InitializeAsync()
        {
            await _Commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            _Commands.Log += Command_Log;
            _Client.MessageReceived += Message_Event;

        }

        private async Task Message_Event(SocketMessage MessageParamater)
        {
            var Message = MessageParamater as SocketUserMessage;
            var Context = new SocketCommandContext(_Client, Message);

            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;

            int ArgPos = 0;
            if (!(Message.HasStringPrefix(Config.bot.Prefix, ref ArgPos) || Message.HasMentionPrefix(_Client.CurrentUser, ref ArgPos))) return;
            var Result = await _Commands.ExecuteAsync(Context, ArgPos, null);

            if(!Result.IsSuccess && Result.Error != CommandError.UnknownCommand)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{DateTime.Now} at Command: {_Commands.Search(Context, ArgPos).Commands[0].Command.Name} in {_Commands.Search(Context, ArgPos).Commands[0].Command.Module.Name} {Result.ErrorReason}");
                Console.ForegroundColor = ConsoleColor.White;
                var Embed = new EmbedBuilder();
                if(Result.ErrorReason == "The input text has too few paramaters")
                {
                    Embed.WithTitle("Error");
                    Embed.WithDescription("This command requires a missing paramater. Please check help command to see what it needs");
                }
                else
                {
                    Embed.WithTitle("Error");
                    Embed.WithDescription(Result.ErrorReason);
                }
                await Context.Channel.SendMessageAsync("",embed: Embed.Build());
            }
        }

        private Task Command_Log(LogMessage Command)
        {
            Console.WriteLine(Command.Message);
            return Task.CompletedTask;
        }
    }
}
