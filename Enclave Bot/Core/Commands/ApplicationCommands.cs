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
    public class ApplicationCommands : ModuleBase<SocketCommandContext>
    {
        public InteractiveService Interactive { get; set; }

        [Command("fapp")]
        [Summary("Application command to become a member of the server")]
        public async Task fapp()
        {
            SocketGuildUser user = Context.User as SocketGuildUser;
            var embed = new EmbedBuilder();
            string[] questions = {
                "Have you, or have you ever been banned from a server/realm? Be honest!",
                "What is your in game name? Case Sensitive please!",
                "Have you put in your gamertag inside of gamer-tag?",
                "What is your age?",
                "Are you a staff member in another, or have been staff in another Discord/Realm?\n-If so, for how long and what were your responsibilities?",
                "List the Factions of this Server, and what is the MOST IMPORTANT RULE about them?",
                "List the 3 ways to get to the Guild Hall",
                "How do you find the realm link once your application is approved? **BE SPECIFIC**",
                "What system are you playing on?",
                "How do you gather gold, and how can you spend it?"
            };
            int counter = 0;
            embed.Title = "Factions Application";
            embed.Description = "Please answer the questions the bot asks you. You have 10 minutes to answer each question\n(Resets everytime you answer a question in time)";
            embed.Color = randomColor();
            try
            {
                await user.SendMessageAsync(embed: embed.Build());
                await Context.Channel.SendMessageAsync("Please check your dm's to answer the application questions");
            }
            catch (Exception ex)
            {
                await ReplyAsync($"An error occurred.\nError Info: {ex}");
                return;
            }

            embed.Description = null;
            var Embed = new EmbedBuilder();
            Embed.Title = $"New Application From {user.Username}";
            Embed.Color = randomColor();

            var builder = new ComponentBuilder()
                .WithButton("Accept", $"Accept:{Context.User.Id}", ButtonStyle.Success)
                .WithButton("Deny", $"Deny:{Context.User.Id}", ButtonStyle.Danger);

            foreach (string question in questions)
            {
                counter++;
                embed.Title = $"{counter}. {question}";
                embed.Color = randomColor();
                await user.SendMessageAsync(embed: embed.Build());
                var answer = await Interactive.NextMessageAsync(x => x.Author.Id == user.Id && x.Channel.GetType().ToString() == "Discord.WebSocket.SocketDMChannel", timeout: TimeSpan.FromMinutes(10));
                if(answer.IsTimeout)
                {
                    await Context.User.SendMessageAsync("Application Timed Out. Execute the command again to redo the application");
                    return;
                }
                Embed.AddField(question, answer.Value.Content);
            }
            await Context.Guild.GetTextChannel(757581056470679672).SendMessageAsync(embed: Embed.Build(),components: builder.Build());
            embed.Title = "Application Successfully Sent";
            await user.SendMessageAsync(embed : embed.Build());
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
