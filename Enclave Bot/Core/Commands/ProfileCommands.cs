using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Discord;
using Enclave_Bot.Core.Database;

namespace Enclave_Bot.Core.Commands
{
    public class ProfileCommands : ModuleBase<SocketCommandContext>
    {
        private Database.Database db = new Database.Database();

        [Command("profile")]
        [Alias("p")]
        [Summary("Displays your profile")]
        public async Task GetProfile()
        {
            var embed = new EmbedBuilder();
            User user = db.GetUserByID(Context.User.Id);
            embed.WithTitle($"{Context.User.Username}'s Profile");
            embed.AddField("Wallet", user.Wallet);
            embed.AddField("Bank", user.Bank);
            embed.AddField("Level", user.Level);
            embed.AddField("XP", user.XP);
            embed.WithThumbnailUrl(Context.User.GetAvatarUrl());

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}
