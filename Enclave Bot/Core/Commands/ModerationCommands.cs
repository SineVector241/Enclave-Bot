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
        private Database.Database db = new Database.Database();
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

        [Command("settings")]
        [Summary("Settings for the guild")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task GuildSettings()
        {
            var guildsettings = db.GetGuildSettingsByID(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithColor(randomColor());
            embed.AddField("Welcome Channel", guildsettings.WelcomeChannel == 0 ? "Not Set" : $"<#{guildsettings.WelcomeChannel}>");
            embed.AddField("Logging Channel", guildsettings.LoggingChannel == 0 ? "Not Set" : $"<#{guildsettings.LoggingChannel}>");
            embed.AddField("Application Channel", guildsettings.ApplicationChannel == 0 ? "Not Set" : $"<#{guildsettings.ApplicationChannel}>");
            embed.AddField("Staff Application Channel", guildsettings.StaffApplicationChannel == 0 ? "Not Set" : $"<#{guildsettings.StaffApplicationChannel}>");
            embed.AddField("Parchment Category", guildsettings.ParchmentCategory == 0 ? "Not Set" : $"<#{guildsettings.ParchmentCategory}>");
            embed.AddField("Verified Role", guildsettings.VerifiedRole == 0 ? "Not Set" : $"<#{guildsettings.VerifiedRole}>");
            embed.AddField("Unverified Role", guildsettings.UnverifiedRole == 0 ? "Not Set" : $"<#{guildsettings.UnverifiedRole}>");
            await ReplyAsync(embed: embed.Build());
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
