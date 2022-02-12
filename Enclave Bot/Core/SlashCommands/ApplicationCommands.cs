using Discord.Interactions;
using Discord.WebSocket;
using Discord;

namespace Enclave_Bot.Core.SlashCommands
{
    public class ApplicationCommands : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private Utils utils = new Utils();
        [SlashCommand("application","Sends either the staff application or the application to get verified")]
        public async Task Application([Choice("Application",1),Choice("Staff Application",2)] int application)
        {
            try
            {
                if (application == 1)
                {
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

                    var embed = new EmbedBuilder()
                        .WithTitle("Enclave Verification Application")
                        .WithColor(utils.randomColor());
                    var selectMenu = new SelectMenuBuilder()
                        .WithPlaceholder("Select a question")
                        .WithCustomId("SelectAppQuestion");

                    for (int i = 0; i < questions.Length; i++)
                    {
                        selectMenu.AddOption($"Question: {i + 1}", $"{i + 1}", $"Answer question {i + 1}");
                        embed.AddField($"{i + 1}. {questions[i]}", "Not Answered");
                    }
                    var builder = new ComponentBuilder()
                        .WithSelectMenu(selectMenu, row: 0)
                        .WithButton("Submit", $"SubmitApp:{Context.Guild.Id}", ButtonStyle.Success, new Emoji("✅"), row: 1);

                    var msg = await Context.User.SendMessageAsync(embed: embed.Build(), components: builder.Build());
                    await RespondAsync(embed: new EmbedBuilder().WithColor(Color.Green).WithTitle("Application Sent").WithDescription($"Please check your DM's to fill out the application\n[Or Click Here]({msg.GetJumpUrl()})").Build());
                }
                else if (application == 2)
                {
                    string[] questions = {
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
                    await RespondAsync("Coming soon");
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
    }
}
