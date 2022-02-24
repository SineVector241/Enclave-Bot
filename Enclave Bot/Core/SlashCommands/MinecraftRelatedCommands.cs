using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Enclave_Bot.Core.SlashCommands
{
    public class MinecraftRelatedCommands : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private Database.Database db = new Database.Database();
        private Utils utils = new Utils();

        [SlashCommand("bounty", "declares a bounty on someone from you")]
        public async Task SetBounty(SocketGuildUser User, string BountyPrize)
        {
            try
            {
                await DeferAsync();
                var gsettings = await db.GetGuildSettingsById(Context.Guild.Id);
                var embed = new EmbedBuilder()
                    .WithTitle($"{Context.User.Username} has set a bounty on {User.Username}!")
                    .WithDescription($"The first person to kill {User.Username} Ingame will get the following prize:\n**{BountyPrize}**")
                    .WithColor(utils.randomColor())
                    .WithThumbnailUrl(User.GetAvatarUrl());
                await Context.Guild.GetTextChannel(gsettings.BountyChannel).SendMessageAsync(embed: embed.Build());
                await FollowupAsync("Successfully sent bounty");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var embed = new EmbedBuilder()
                    .WithTitle("An error has occured")
                    .WithDescription($"Error Message: {ex.Message}")
                    .WithColor(Color.DarkRed);
                await FollowupAsync(embed: embed.Build());
            }
        }

        [SlashCommand("serverpe", "Pings a minecraft bedrock server")]
        public async Task McbeServerPing(string IP, int Port)
        {
            try
            {
                await DeferAsync();
                var serverresponse = utils.GetRequest($"https://api.bedrockinfo.com/v2/status?server={IP}&port={Port}");
                dynamic Data = JsonConvert.DeserializeObject(serverresponse);
                Regex r = new Regex(@"§.");
                string servername = Data["ServerName"];
                var embed = new EmbedBuilder()
                    .WithTitle($"**{IP}:{Port}** information")
                    .AddField("ServerName", $"{r.Replace(servername,"")}")
                    .AddField("HostName", $"{Data["HostName"]}")
                    .AddField("Gamemode", $"{Data["GameMode"]}")
                    .AddField("Players", $"{Data["Players"]}/{Data["MaxPlayers"]}")
                    .WithColor(utils.randomColor());
                await FollowupAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var embed = new EmbedBuilder()
                    .WithTitle("An error has occured")
                    .WithDescription($"Error Message: {ex.Message}")
                    .WithColor(Color.DarkRed);
                await FollowupAsync(embed: embed.Build());
            }
        }
    }
}