using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;

namespace Enclave_Bot.Core.SlashCommands
{
    public class Miscellaneous : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        Utils utils = new Utils();

        [SlashCommand("ping", "Latency of the bot")]
        public async Task Ping()
        {
            try
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "Pong!",
                    Description = $"Bots ping is: {Context.Client.Latency} ms",
                    Color = Context.Client.Latency <= 100 ? Color.Green : Color.Red,
                    Footer = new EmbedFooterBuilder().WithText(Context.User.Username).WithIconUrl(Context.User.GetAvatarUrl())
                };

                await RespondAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var embed = new EmbedBuilder()
                    .WithTitle("An error has occured")
                    .WithDescription($"Error Message: {ex.Message}")
                    .WithColor(Color.DarkRed);
                await RespondAsync(embed: embed.Build());
            }
        }

        [SlashCommand("joke", "Sends a random joke")]
        public async Task Joke()
        {
            try
            {
                var data = utils.GetRequest("https://v2.jokeapi.dev/joke/Any?blacklistFlags=nsfw,religious,political,racist,sexist");
                dynamic Data = JsonConvert.DeserializeObject(data);
                EmbedBuilder embed = new EmbedBuilder();

                if (Data["type"] == "twopart")
                {
                    embed.Title = Data["setup"];
                    embed.Description = $"||{Data["delivery"]}||";
                    embed.Color = Color.Blue;
                    await RespondAsync(embed: embed.Build());
                }
                else if (Data["type"] == "single")
                {
                    embed.Title = "Single Joke";
                    embed.Description = $"{Data["joke"]}";
                    embed.Color = Color.Blue;
                    await RespondAsync(embed: embed.Build());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var embed = new EmbedBuilder()
                    .WithTitle("An error has occured")
                    .WithDescription($"Error Message: {ex.Message}")
                    .WithColor(Color.DarkRed);
                await RespondAsync(embed: embed.Build());
            }
        }

        [SlashCommand("cooldowns","Check your cooldowns")]
        public async Task Cooldowns()
        {
            int beg = utils.CheckCooldown(Context.User, "Beg", 30).Seconds;
            int deposit = utils.CheckCooldown(Context.User, "Deposit", 60).Seconds;
            int withdraw = utils.CheckCooldown(Context.User, "Withdraw", 60).Seconds;
            int steal = utils.CheckCooldown(Context.User, "Steal", 60).Seconds;
            int stolen = utils.CheckCooldown(Context.User, "Stolen", 300).Seconds;
            int work = utils.CheckCooldown(Context.User, "Work", 3600).Seconds;
            int jobhire = utils.CheckCooldown(Context.User, "QuitJob", 3600).Seconds;
            var embed = new EmbedBuilder()
                .WithTitle($"{Context.User.Username}'s cooldowns")
                .AddField("Beg", $"{ (beg <= 0 ? "Ready" : beg + " Seconds")}")
                .AddField("Deposit", $"{(deposit <= 0 ? "Ready" : deposit + " Seconds")}")
                .AddField("Withdraw", $"{(withdraw <= 0 ? "Ready" : withdraw + " Seconds")}")
                .AddField("Steal", $"{(steal <= 0 ? "Ready" : steal + " Seconds")}")
                .AddField("Can Be Stolen From", $"{(stolen <= 0 ? "Ready" : stolen + " Seconds")}")
                .AddField("Work", $"{(work <= 0 ? "Ready" : work + " Seconds")}")
                .AddField("JobHire", $"{(jobhire <= 0 ? "Ready" : jobhire + " Seconds")}")
                .WithColor(utils.randomColor());
            await RespondAsync(embed: embed.Build());
        }
    }
}
