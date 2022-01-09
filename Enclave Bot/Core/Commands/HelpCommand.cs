using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Reflection;

namespace Enclave_Bot.Core.Commands
{
    public class HelpCommand : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _Service;
        public HelpCommand(CommandService Service)
        {
            _Service = Service;
        }

        [Command("help")]
        [Alias("command","commands")]
        [Summary("Display all commands or displays a specific command")]
        public async Task Help([Remainder] string Command = "")
        {
            string prefix = Config.bot.Prefix;
            var embed = new EmbedBuilder()
            {
                Color = Color.Orange,
                Description = "These are the available commands"
            };
            if (Command == "")
            {
                foreach(var module in _Service.Modules)
                {
                    string description = "";
                    foreach(var cmd in module.Commands)
                    {
                        description += $"{prefix}{cmd.Aliases.First()}\n";
                    }

                    if(!string.IsNullOrWhiteSpace(description))
                    {
                        string name = module.Name;
                        embed.AddField(name, description,false);
                    }
                }
                await ReplyAsync(embed: embed.Build());
            }
            else
            {
                var result = _Service.Search(Context, Command);
                if(!result.IsSuccess)
                {
                    await ReplyAsync($"Error: Could not find **{Command}**");
                }

                var Embed = new EmbedBuilder()
                {
                    Color = Color.Blue,
                    Description = $"Similar Commands To **{Command}**"
                };

                foreach(var match in result.Commands)
                {
                    var cmd = match.Command;
                    Embed.AddField(x =>
                    {
                        x.Name = String.Join(", ", cmd.Aliases);
                        x.Value = $"Paramaters {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                        $"Summary {cmd.Summary}";
                        x.IsInline = false;
                    });
                }
                await ReplyAsync(embed: Embed.Build());
            }
        }
    }
}
