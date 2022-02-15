using Discord.Interactions;
using Discord.WebSocket;
using Discord;

namespace Enclave_Bot.Core.SlashCommands
{
    public class Economy : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private Database.Database db = new Database.Database();
        private Utils utils = new Utils();
        [SlashCommand("balance", "Displays yours or another users bank and wallet balance")]
        public async Task Balance([Summary("User", "User you want to target")] SocketGuildUser User = null)
        {
            try
            {
                if (User == null)
                {
                    if (await db.UserHasProfile(Context.User.Id))
                    {
                        var profile = await db.GetUserProfileById(Context.User.Id);
                        var embed = new EmbedBuilder()
                            .WithTitle($"{Context.User.Username}'s balance")
                            .AddField("Wallet", profile.Wallet.ToString())
                            .AddField("Bank", profile.Bank.ToString())
                            .WithColor(utils.randomColor());
                        await RespondAsync(embed: embed.Build());
                    }
                    else
                    {
                        await RespondAsync("Please create an account first! */createaccount*");
                    }
                }
                else
                {
                    if (await db.UserHasProfile(User.Id))
                    {
                        var profile = await db.GetUserProfileById(User.Id);
                        var embed = new EmbedBuilder()
                            .WithTitle($"{User.Username}'s balance")
                            .AddField("Wallet", profile.Wallet.ToString())
                            .AddField("Bank", profile.Bank.ToString())
                            .WithColor(utils.randomColor());
                        await RespondAsync(embed: embed.Build());
                    }
                    else
                    {
                        await RespondAsync("This user does not have an account created!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var embed = new EmbedBuilder()
                    .WithTitle("An error has occured")
                    .WithDescription($"Error Message: {ex.Message}")
                    .WithColor(Color.DarkRed);
                await Context.Channel.SendMessageAsync(embed: embed.Build());
            }
        }

        [SlashCommand("rank", "Displays yours or someone elses XP and Level")]
        public async Task Rank([Summary("User", "User you want to target")] SocketGuildUser User = null)
        {
            try
            {
                if (User == null)
                {
                    if (await db.UserHasProfile(Context.User.Id))
                    {
                        var profile = await db.GetUserProfileById(Context.User.Id);
                        var embed = new EmbedBuilder()
                            .WithTitle($"{Context.User.Username}'s rank")
                            .AddField("Level", profile.Level.ToString())
                            .AddField("XP", profile.XP.ToString())
                            .WithColor(utils.randomColor());
                        await RespondAsync(embed: embed.Build());
                    }
                    else
                    {
                        await RespondAsync("Please create an account first! */createaccount*");
                    }
                }
                else
                {
                    if (await db.UserHasProfile(User.Id))
                    {
                        var profile = await db.GetUserProfileById(User.Id);
                        var embed = new EmbedBuilder()
                            .WithTitle($"{User.Username}'s rank")
                            .AddField("Level", profile.Level.ToString())
                            .AddField("XP", profile.XP.ToString())
                            .WithColor(utils.randomColor());
                        await RespondAsync(embed: embed.Build());
                    }
                    else
                    {
                        await RespondAsync("This user does not have an account created!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var embed = new EmbedBuilder()
                    .WithTitle("An error has occured")
                    .WithDescription($"Error Message: {ex.Message}")
                    .WithColor(Color.DarkRed);
                await Context.Channel.SendMessageAsync(embed: embed.Build());
            }
        }

        [SlashCommand("createaccount", "Creates your profile in the bot")]
        public async Task CreateProfile()
        {
            try
            {
                if (!await db.UserHasProfile(Context.User.Id))
                {
                    await db.CreateUserProfile(new Database.UserProfile { UserID = Context.User.Id, Wallet = 0, Bank = 200, XP = 0, Level = 0 });
                    await RespondAsync("Created your account!");
                }
                else
                {
                    await RespondAsync("You already have an account!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var embed = new EmbedBuilder()
                    .WithTitle("An error has occured")
                    .WithDescription($"Error Message: {ex.Message}")
                    .WithColor(Color.DarkRed);
                await Context.Channel.SendMessageAsync(embed: embed.Build());
            }
        }
    }
}
