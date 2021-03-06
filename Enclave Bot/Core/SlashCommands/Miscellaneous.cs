using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;
using Fergun.Interactive;

namespace Enclave_Bot.Core.SlashCommands
{
    public class Miscellaneous : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        Utils utils = new Utils();
        public InteractiveService Interactive { get; set; }
        private Database.Database db = new Database.Database();

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

        [SlashCommand("cooldowns", "Check your cooldowns")]
        public async Task Cooldowns()
        {
            try
            {
                int beg = utils.CheckCooldown(Context.User, "Beg").Seconds;
                int deposit = utils.CheckCooldown(Context.User, "Deposit").Seconds;
                int withdraw = utils.CheckCooldown(Context.User, "Withdraw").Seconds;
                int steal = utils.CheckCooldown(Context.User, "Steal").Seconds;
                int stolen = utils.CheckCooldown(Context.User, "Stolen").Seconds;
                int work = utils.CheckCooldown(Context.User, "Work").Seconds;
                int jobhire = utils.CheckCooldown(Context.User, "QuitJob").Seconds;
                int mine = utils.CheckCooldown(Context.User, "Mine").Seconds;
                int XP = utils.CheckCooldown(Context.User, "XP").Seconds;
                int Suggestion = utils.CheckCooldown(Context.User, "Suggestion").Seconds;
                var embed = new EmbedBuilder()
                    .WithTitle($"{Context.User.Username}'s cooldowns")
                    .AddField("Beg", $"{ (beg <= 0 ? "Ready" : beg + " Seconds")}")
                    .AddField("Deposit", $"{(deposit <= 0 ? "Ready" : deposit + " Seconds")}")
                    .AddField("Withdraw", $"{(withdraw <= 0 ? "Ready" : withdraw + " Seconds")}")
                    .AddField("Steal", $"{(steal <= 0 ? "Ready" : steal + " Seconds")}")
                    .AddField("Can Be Stolen From", $"{(stolen <= 0 ? "Ready" : stolen + " Seconds")}")
                    .AddField("Work", $"{(work <= 0 ? "Ready" : work + " Seconds")}")
                    .AddField("JobHire", $"{(jobhire <= 0 ? "Ready" : jobhire + " Seconds")}")
                    .AddField("Mine", $"{(mine <= 0 ? "Ready" : mine + " Seconds")}")
                    .AddField("XPCooldown", $"{(XP <= 0 ? "Ready" : XP + " Seconds")}")
                    .AddField("Suggestion", $"{(Suggestion <= 0 ? "Ready" : Suggestion + " Seconds")}")
                    .WithColor(utils.randomColor());
                await RespondAsync(embed: embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [SlashCommand("poll", "Create a poll. Use | to separate answers")]
        public async Task Poll(string question, string options)
        {
            try
            {
                await DeferAsync();
                string[] regs = { "🇦", "🇧", "🇨", "🇩", "🇪", "🇫" };
                string[] listOptions = options.Split("|");
                var embed = new EmbedBuilder()
                    .WithColor(utils.randomColor())
                    .WithTitle(question);
                string content = "";
                for (int i = 0; i < listOptions.Length; i++)
                {
                    content += $"{regs[i]}**: {listOptions[i]}**\n";
                }
                embed.WithDescription(content);
                var msg = await FollowupAsync(embed: embed.Build());
                for (int i = 0; i < listOptions.Length; i++)
                {
                    await msg.AddReactionAsync(new Emoji(regs[i]));
                }
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

        [SlashCommand("suggest", "Make a suggestion")]
        public async Task Suggest(string suggestion)
        {
            try
            {
                await DeferAsync();
                var cooldown = utils.Cooldown(Context.User, "Suggestion", 60);
                if(!cooldown.CooledDown)
                {
                    await RespondAsync($"You are on cooldown for this command. Try again in {cooldown.Seconds}");
                    return;
                }
                var guildsettings = await db.GetGuildSettingsById(Context.Guild.Id);
                var channel = Context.Guild.GetTextChannel(guildsettings.SuggestionsChannel);
                var embed = new EmbedBuilder()
                    .WithTitle($"New suggestion from {Context.User.Username}")
                    .WithDescription(suggestion)
                    .WithColor(utils.randomColor())
                    .WithThumbnailUrl(Context.User.GetAvatarUrl())
                    .WithTimestamp(DateTime.Now);
                var msg = await channel.SendMessageAsync(embed: embed.Build());
                await msg.AddReactionAsync(new Emoji("✅"));
                await msg.AddReactionAsync(new Emoji("❌"));
                await FollowupAsync("Successfully sent suggestion", ephemeral: true);
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
