using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Fergun.Interactive;
using Discord.WebSocket;

namespace Enclave_Bot.Core.Commands
{
    public class Miscellaneous : ModuleBase<SocketCommandContext>
    {
        public InteractiveService Interactive { get; set; }

        [Command("ping")]
        [Alias("latency")]
        [Summary("Displays the bots current latency")]
        public async Task PingCommand()
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Pong!",
                Description = $"Bots ping is: {Context.Client.Latency} ms",
                Color = Context.Client.Latency <= 100 ? Color.Green : Color.Red,
                Footer = new EmbedFooterBuilder().WithText(Context.User.Username).WithIconUrl(Context.User.GetAvatarUrl())
            };

            await Context.Channel.SendMessageAsync(embed: embed.Build());
        }

        [Command("joke")]
        [Summary("Sends a random joke")]
        public async Task Joke()
        {
            try
            {
                WebRequest request = WebRequest.Create("https://v2.jokeapi.dev/joke/Any?blacklistFlags=nsfw,religious,political,racist,sexist");
                request.Method = "GET";
                using WebResponse webResponse = request.GetResponse();
                using Stream webStream = webResponse.GetResponseStream();

                using StreamReader reader = new StreamReader(webStream);
                string data = reader.ReadToEnd();
                JObject jsonData = JObject.Parse(data);
                dynamic Data = JsonConvert.DeserializeObject(data);
                EmbedBuilder embed = new EmbedBuilder();

                if (Data["type"] == "twopart")
                {
                    embed.Title = Data["setup"];
                    embed.Description = $"||{Data["delivery"]}||";
                    embed.Color = Color.Blue;
                    await Context.Channel.SendMessageAsync(embed: embed.Build());
                }
                else if (Data["type"] == "single")
                {
                    embed.Title = "Single Joke";
                    embed.Description = $"{Data["joke"]}";
                    embed.Color = Color.Blue;
                    await Context.Channel.SendMessageAsync(embed: embed.Build());
                }
            }
            catch (Exception e)
            {
                Log.Error(String.Format("{0} - {1}", e.InnerException?.Message ?? e.Message, e.StackTrace));
                await Context.Channel.SendMessageAsync("An Error Occurred");
            }
        }

        [Command("embed")]
        [Summary("Sends an embed message")]
        public async Task Embedder(string Title, [Remainder] string Description = null)
        {
            var embed = new EmbedBuilder() { Title = Title, Description = Description, Color = randomColor() };
            await Context.Channel.SendMessageAsync(embed: embed.Build());
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
