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
            try
            {
                int counter = 0;
                var embed = new EmbedBuilder()
                    .WithColor(randomColor())
                    .WithTitle("Faction Application");
                var selectMenu = new SelectMenuBuilder()
                    .WithCustomId("fapp")
                    .WithMaxValues(1)
                    .WithMinValues(1)
                    .WithPlaceholder("Select a question");
                var builder = new ComponentBuilder();

                string[] questions = {
                "Have you ever been banned from a server/realm? Be honest!",
                "What is your in game name? Case Sensitive please!",
                "Have you put in your gamertag in the gamer-tag channel?",
                "What is your age?",
                "Are you, or have you ever been a staff member in another Discord/Realm?  -If so, for how long and what were your responsibilities?",
                "List the Factions of this Server, and what is the MOST IMPORTANT RULE about them?",
                "List the 3 ways to get to the Guild Hall",
                "How do you find the realm link once your application is approved? **BE SPECIFIC**",
                "What platform do you usually play on?",
                "How do you gather gold, and how can you spend it?"
            };

                for (int i = 1; i <= questions.Length; i++)
                {
                    selectMenu.AddOption($"Question {i}", $"{i}.", $"Fillout Question {i}");
                }

                foreach (string question in questions)
                {
                    counter++;
                    embed.AddField($"{counter}. {question}", "Not Answered");
                }

                selectMenu.AddOption("Submit", $"Submit:{Context.Guild.Id}", "Submits the application", new Emoji("✅"));
                builder.WithSelectMenu(selectMenu, row: 0);

                var msg = await Context.User.SendMessageAsync(embed: embed.Build(), components: builder.Build());
                await Context.Message.ReplyAsync(embed: new EmbedBuilder().WithColor(randomColor()).WithTitle("Application Sent").WithDescription($"Please check your DM's to fill out the application\n[Or Click Here]({msg.GetJumpUrl()})").Build());
            }
            catch
            {
                await Context.Channel.SendMessageAsync("Error. Could not send message. Please make sure your settings allow direct messages from anyone");
            }
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
            await Context.Channel.SendMessageAsync("Coming soon");
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
