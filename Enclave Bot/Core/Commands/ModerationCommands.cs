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

        [Command("rrdash")]
        [Summary("Reaction Role Dashboard")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ReactionRoleDash(SocketTextChannel channel)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Reaction Role Setup")
                .WithDescription("Dashboard for setting up reaction roles")
                .WithColor(randomColor());
            var builder = new ComponentBuilder()
                .WithButton("Create", $"RRDashAdd:{Context.User.Id}", ButtonStyle.Primary, new Emoji("➕"))
                .WithButton("Remove", $"RRDashRemove:{Context.User.Id}", ButtonStyle.Danger, new Emoji("➖"))
                .WithButton("Send Reaction Setup", $"SendRR:{channel.Id},{Context.User.Id}", ButtonStyle.Success, new Emoji("✅"));

            await Context.Channel.SendMessageAsync(embed: embed.Build(), components: builder.Build());
        }

        Random rnd = new Random();
        private Color randomColor()
        {
            Color randomColor = new Color(GenerateRandomInt(rnd), GenerateRandomInt(rnd), GenerateRandomInt(rnd));
            return randomColor;
        }
        public static int GenerateRandomInt(Random rnd)
        {
            return rnd.Next(256);
        }
    }
}
