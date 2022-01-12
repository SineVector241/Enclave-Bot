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
                Embed.AddField($"{counter}. {question}", answer.Value.Content);
            }
            await Context.Guild.GetTextChannel(757581056470679672).SendMessageAsync(embed: Embed.Build(),components: builder.Build());
            embed.Title = "Application Successfully Sent";
            await user.SendMessageAsync(embed : embed.Build());
        }

        [Command("staffapp")]
        [Alias("sapp")]
        [Summary("Staff application")]
        public async Task staffapp()
        {
            SocketGuildUser User = Context.User as SocketGuildUser;
            string[] questions =
            {
                "What is your gamertag?",
                "What is your age?",
                "Do you have any previous experience in moderating for a discord/Minecraft server? \nIf so, please state what you did and what the discord server was focused around.",
                "For how long have you been a member of E.K?(Enclave Kingdoms) \n**Estimating is fine**",
                "What could you do to benefit Enclave Kingdoms if you were to be chosen to be apart of the White Guard?",
                "Do you have any background knowledge in Command blocks, add-ons, etc?",
                "Why do you want to be apart of the White Guard and what are your interests in being staff?",
                "Have you memorized all the rules and guidelines of Enclave Kingdoms? Check out <#757596592583868416> for information regarding the rules and guidelines of Enclave Kingdoms. Note: the king who is interviewing you may ask any questions regarding the <#757596592583868416> to ensure you have memorized them.",
                "How active would you be if chosen to be apart the White Guard? \nNote: we require considerable activity for members of the White Guard"
            };
            int counter = 0;
            var embed = new EmbedBuilder() { Title = "**STAFF APPLICATION**", Color = randomColor(), Description = "*Please be as detailed as possible when filling out this application, the more detailed it is, the better this application looks to the admins, we all will peruse over it*\nYou have 10 minutes to answer each question(Resets everytime you answer one)"};
            var Embed = new EmbedBuilder() { Title = $"New staff application from: {User.Username}" };
            var builder = new ComponentBuilder().WithButton("Accept", $"SAccept:{Context.User.Id}", ButtonStyle.Success).WithButton("Deny", $"SDeny:{Context.User.Id}", ButtonStyle.Danger);
            try
            {
                await User.SendMessageAsync(embed: embed.Build());
                await Context.Channel.SendMessageAsync("Please check your dm's to answer the application questions");
            }
            catch (Exception ex)
            {
                await ReplyAsync($"An error occurred.\nError Info: {ex}");
                return;
            }

            foreach (string question in questions)
            {
                counter++;
                embed.Description = question;
                embed.Title = $"Question: {counter}";
                await User.SendMessageAsync(embed: embed.Build());
                var answer = await Interactive.NextMessageAsync(x => x.Author.Id == User.Id && x.Channel.GetType().ToString() == "Discord.WebSocket.SocketDMChannel", timeout: TimeSpan.FromMinutes(10));
                Embed.AddField(counter.ToString(), $"**{question}**\n{answer.Value.Content}");
                if (answer.IsTimeout)
                {
                    await Context.User.SendMessageAsync("Application Timed Out. Execute the command again to redo the application");
                    return;
                }
            }
            await Context.Guild.GetTextChannel(927282638996242432).SendMessageAsync(embed: Embed.Build(), components: builder.Build());
            embed.Title = "Application Successfully Sent";
            embed.Description = null;
            await User.SendMessageAsync(embed: embed.Build());
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
