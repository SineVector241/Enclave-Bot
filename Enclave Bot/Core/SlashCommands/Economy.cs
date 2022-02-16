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

        [SlashCommand("beg", "Random amount of money you get given by a stranger")]
        public async Task Beg()
        {
            try
            {
                var cooldown = utils.Cooldown(new UserCooldown() { CooldownType = "Beg", UserID = Context.User.Id }, 30);
                if (cooldown.CooledDown)
                {
                    if (await db.UserHasProfile(Context.User.Id))
                    {
                        var profile = await db.GetUserProfileById(Context.User.Id);
                        int rnd = new Random().Next(30);
                        profile.Wallet += rnd;
                        await db.UpdateUserProfile(profile);
                        await RespondAsync($"A random person gave you ${rnd}. You now have {profile.Wallet} in your wallet");
                    }
                    else
                    {
                        await RespondAsync("Please create an account first! */createaccount*");
                    }
                }
                else
                {
                    await RespondAsync($"You are on cooldown for this command! Try again in {cooldown.Seconds} seconds");
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

        [SlashCommand("deposit", "Deposits all or a set amount of money into your bank")]
        public async Task Deposit([Summary("Amount", "The amount you want to deposit")] int Amount = 0)
        {
            try
            {
                var cooldown = utils.Cooldown(new UserCooldown() { CooldownType = "Deposit", UserID = Context.User.Id }, 60);
                if (cooldown.CooledDown)
                {
                    if (await db.UserHasProfile(Context.User.Id))
                    {
                        var profile = await db.GetUserProfileById(Context.User.Id);
                        if (Amount == 0)
                        {
                            int wallet = profile.Wallet;
                            profile.Bank += profile.Wallet;
                            profile.Wallet = 0;
                            await db.UpdateUserProfile(profile);
                            await RespondAsync($"Deposited ${wallet}(All) into your bank");
                        }
                        else
                        {
                            if(profile.Wallet < Amount)
                            {
                                await RespondAsync("You don't have that much money in your wallet!");
                            }
                            else
                            {
                                profile.Bank += Amount;
                                profile.Wallet -= Amount;
                                await db.UpdateUserProfile(profile);
                                await RespondAsync($"Deposited ${Amount} into your bank. You now have ${profile.Wallet} in your wallet");
                            }
                        }
                    }
                    else
                    {
                        await RespondAsync("Please create an account first! */createaccount*");
                    }
                }
                else
                {
                    await RespondAsync($"You are on cooldown for this command! Try again in {cooldown.Seconds} seconds");
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

        [SlashCommand("withdraw", "Withdraws all or a set amount of money into your wallet")]
        public async Task Withdraw([Summary("Amount", "The amount you want to withdraw")] int Amount = 0)
        {
            try
            {
                var cooldown = utils.Cooldown(new UserCooldown() { CooldownType = "Deposit", UserID = Context.User.Id }, 60);
                if (cooldown.CooledDown)
                {
                    if (await db.UserHasProfile(Context.User.Id))
                    {
                        var profile = await db.GetUserProfileById(Context.User.Id);
                        if (Amount == 0)
                        {
                            int bank = profile.Bank;
                            profile.Wallet += profile.Bank;
                            profile.Bank = 0;
                            await db.UpdateUserProfile(profile);
                            await RespondAsync($"Withdrawed ${bank}(All) into your wallet");
                        }
                        else
                        {
                            if (profile.Bank < Amount)
                            {
                                await RespondAsync("You don't have that much money in your bank!");
                            }
                            else
                            {
                                profile.Wallet += Amount;
                                profile.Bank -= Amount;
                                await db.UpdateUserProfile(profile);
                                await RespondAsync($"Withdrawed ${Amount} into your wallet. You now have ${profile.Wallet} in your wallet");
                            }
                        }
                    }
                    else
                    {
                        await RespondAsync("Please create an account first! */createaccount*");
                    }
                }
                else
                {
                    await RespondAsync($"You are on cooldown for this command! Try again in {cooldown.Seconds} seconds");
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
