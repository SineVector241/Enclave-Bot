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
        private Database.Database db = new Database.Database();

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

        [Command("purge")]
        [Alias("clear")]
        [Summary("Clears text from a channel")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeMessages(int amount)
        {
            SocketTextChannel channel = Context.Channel as SocketTextChannel;
            var msgs = await channel.GetMessagesAsync(limit: amount).FlattenAsync();
            await Context.Message.DeleteAsync();
            await channel.DeleteMessagesAsync(msgs);
            await Interactive.DelayedSendMessageAndDeleteAsync(channel, null, TimeSpan.FromSeconds(5),null,$"Deleted **{amount}** Messages");
        }

        [Command("kick")]
        [Alias("boot")]
        [Summary("Kicks a user from the guild")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUser(SocketGuildUser user, string reason = null)
        {
            await user.KickAsync(reason);
            await Context.Channel.SendMessageAsync($"**Kicked User:** {user.Username} \n**Reason:** {reason}");
        }

        [Command("ban")]
        [Alias("hammer")]
        [Summary("Kicks a user from the guild")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUser(SocketGuildUser user, string reason = null)
        {
            await user.BanAsync(1,reason);
            await Context.Channel.SendMessageAsync($"**Banned User:** {user.Username} \n**Reason:** {reason}");
        }

        [Command("settings")]
        [Summary("Settings for the guild")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task GuildSetting()
        {
            var settings = await db.GetGuildSettingsById(Context.Guild.Id);
            var embed = new EmbedBuilder()
                .WithColor(randomColor());
            embed.AddField("Welcome Channel", settings.WelcomeChannel == 0 ? "Not Set" : $"<#{settings.WelcomeChannel}>");
            embed.AddField("Logging Channel", settings.LoggingChannel == 0 ? "Not Set" : $"<#{settings.LoggingChannel}>");
            embed.AddField("Application Channel", settings.ApplicationChannel == 0 ? "Not Set" : $"<#{settings.ApplicationChannel}>");
            embed.AddField("Staff Application Channel", settings.StaffApplicationChannel == 0 ? "Not Set" : $"<#{settings.StaffApplicationChannel}>");
            embed.AddField("Parchment Category", settings.ParchmentCategory == 0 ? "Not Set" : $"<#{settings.ParchmentCategory}>");
            embed.AddField("Verified Role", settings.VerifiedRole == 0 ? "Not Set" : $"<#{settings.VerifiedRole}>");
            embed.AddField("Unverified Role", settings.UnverifiedRole == 0 ? "Not Set" : $"<#{settings.UnverifiedRole}>");
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
