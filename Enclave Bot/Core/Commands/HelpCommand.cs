using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System.Reflection;
using Serilog;

namespace Enclave_Bot.Core.Commands
{
    public class HelpCommand : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        public HelpCommand(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        [Alias("command", "commands")]
        [Summary("Display all commands or displays a specific command")]
        public async Task Help([Remainder] string command = "")
        {
            try
            {
                string prefix = Config.BotConfiguration.Prefix;
                EmbedBuilder defaultHelpEmbedBuilder = new EmbedBuilder()
                {
                    Color = Color.Orange,
                    Description = "These are the available commands"
                };
                EmbedBuilder commandHelpEmbedBuilder = new EmbedBuilder()
                {
                    Color = Color.Blue,
                    Description = $"Similar Commands To **{command}**"
                };

                if (string.IsNullOrWhiteSpace(command))
                {
                    foreach (ModuleInfo module in _service.Modules)
                    {
                        string description = "";
                        foreach (CommandInfo cmd in module.Commands)
                        {
                            description += $"{prefix}{cmd.Aliases.First()}\n";
                        }

                        if (!string.IsNullOrWhiteSpace(description))
                        {
                            string name = module.Name;
                            defaultHelpEmbedBuilder.AddField(name, description, false);
                        }
                    }
                    await ReplyAsync(embed: defaultHelpEmbedBuilder.Build());
                }
                else
                {
                    SearchResult result = _service.Search(Context, command);

                    if (!result.IsSuccess)
                    {
                        await ReplyAsync($"Error: Could not find command **{command}**");
                    }

                    foreach (CommandMatch match in result.Commands)
                    {
                        CommandInfo cmd = match.Command;
                        commandHelpEmbedBuilder.AddField(x =>
                        {
                            x.Name = String.Join(", ", cmd.Aliases);
                            x.Value = $"Paramaters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                            $"Summary: {cmd.Summary}";
                            x.IsInline = false;
                        });
                    }
                    await ReplyAsync(embed: commandHelpEmbedBuilder.Build());
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("{0} - {1}", e.InnerException?.Message ?? e.Message, e.StackTrace));
                await Context.Channel.SendMessageAsync("An Error Occurred");
            }
        }
    }
}