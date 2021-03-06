using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using Fergun.Interactive;

namespace Enclave_Bot.Core.SlashCommands
{
    public class Economy : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        private Database.Database db = new Database.Database();
        private Utils utils = new Utils();
        public InteractiveService Interactive { get; set; }

        [SlashCommand("createaccount", "Creates your profile in the bot")]
        public async Task CreateProfile()
        {
            try
            {
                if (!await db.UserHasProfile(Context.User.Id))
                {
                    await db.CreateUserProfile(new Database.UserProfile { UserID = Context.User.Id, Wallet = 0, Bank = 200, XP = 0, Level = 0, WorkType = "None" });
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

        [SlashCommand("beg", "Random amount of money you get given by a stranger")]
        public async Task Beg()
        {
            try
            {
                var cooldown = utils.Cooldown(Context.User, "Beg", 30);
                if (cooldown.CooledDown)
                {
                    if (await db.UserHasProfile(Context.User.Id))
                    {
                        var profile = await db.GetUserProfileById(Context.User.Id);
                        int rnd = new Random().Next(30);
                        profile.Wallet += rnd;
                        await db.UpdateUserProfile(profile);
                        await RespondAsync($"A random person gave you ${rnd}. You now have ${profile.Wallet} in your wallet");
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
                if(Amount < 0)
                {
                    await RespondAsync("Amount must higher than $0");
                    return;
                }
                var cooldown = utils.Cooldown(Context.User, "Deposit", 60);
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
                            if (profile.Wallet < Amount)
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
                if (Amount < 0)
                {
                    await RespondAsync("Amount must higher than $0");
                    return;
                }
                var cooldown = utils.Cooldown(Context.User, "Withdraw", 60);
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

        [SlashCommand("steal", "Steal a certain amount of money from a player. 20% chance to be caught")]
        public async Task Steal(SocketGuildUser User)
        {
            try
            {
                if (User.Id == Context.User.Id)
                {
                    await RespondAsync("You can't steal from yourself dummy");
                    return;
                }
                int rnd = new Random().Next(1, 10);
                if (!await db.UserHasProfile(Context.User.Id))
                {
                    await RespondAsync("Please create an account first! */createaccount*");
                    return;
                }

                if (!await db.UserHasProfile(User.Id))
                {
                    await RespondAsync("This user does not have an account!");
                    return;
                }

                var cooldown = utils.Cooldown(Context.User, "Steal", 60);
                if (!cooldown.CooledDown)
                {
                    await RespondAsync($"You are on cooldown for this command! Try again in {cooldown.Seconds} seconds");
                    return;
                }

                var cooldownvictim = utils.Cooldown(User, "Stolen", 300);
                if (!cooldownvictim.CooledDown)
                {
                    await RespondAsync("This user has already been stolen from in the past 5 minutes. Give it a break!");
                    return;
                }

                var stealer = await db.GetUserProfileById(Context.User.Id);
                var victim = await db.GetUserProfileById(User.Id);

                if (stealer.Wallet < 100)
                {
                    await RespondAsync("You must have atleast $100 in your wallet before you steal someone. Get a good meal before stealing");
                    return;
                }

                if (victim.Wallet < 100)
                {
                    await RespondAsync("This user does not have $100 in their wallet. It's not worth it man");
                    return;
                }

                utils.Cooldown(User, "Stolen", 300);
                if (rnd > 2)
                {
                    int amount = new Random().Next(1, victim.Wallet);
                    stealer.Wallet += amount;
                    victim.Wallet -= amount;
                    await RespondAsync($"Noice steal. You stole ${amount} from {User.Mention}");
                }
                else
                {
                    stealer.Wallet -= 100;
                    victim.Wallet += 100;
                    await RespondAsync($"LOLS. You got caught trying to pickpocket {User.Mention}'s wallet and payed a fine of $100 to ${User.Mention}");
                }
                await db.UpdateUserProfile(victim);
                await db.UpdateUserProfile(stealer);
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

        [SlashCommand("work", "Work for your job per hour")]
        public async Task Work()
        {
            try
            {
                if (!await db.UserHasProfile(Context.User.Id))
                {
                    await RespondAsync("Please create an account first! */createaccount*");
                    return;
                }
                var profile = await db.GetUserProfileById(Context.User.Id);
                if (profile.WorkType == "None")
                {
                    await RespondAsync("You don't have a job! select one using /jobs");
                    return;
                }
                var cooldown = utils.Cooldown(Context.User, "Work", 3600);
                if (!cooldown.CooledDown)
                {
                    await RespondAsync($"You are on cooldown for this command! Try again in {cooldown.Seconds} seconds");
                    return;
                }

                await DeferAsync();
                int payamount = 0;
                switch (profile.WorkType)
                {
                    case "DiscordMod":
                        payamount = 200;
                        break;
                    case "DiscordAdmin":
                        payamount = 400;
                        break;
                }

                bool success = false;
                switch (new Random().Next(1, 2))
                {
                    case 1:
                        string[] emotes = { "unamused", "joy", "smile", "smirk", "rofl", "scream", "rage", "sunglasses", "innocent", "grimacing" };
                        int randomemote = new Random().Next(0, emotes.Count());
                        var builder = new ComponentBuilder();
                        builder.WithButton("😒", "unamused");
                        builder.WithButton("😂", "joy");
                        builder.WithButton("😄", "smile");
                        builder.WithButton("😏", "smirk");
                        builder.WithButton("🤣", "rofl");
                        builder.WithButton("😱", "scream", row: 1);
                        builder.WithButton("😡", "rage", row: 1);
                        builder.WithButton("😎", "sunglasses", row: 1);
                        builder.WithButton("😇", "innocent", row: 1);
                        builder.WithButton("😬", "grimacing", row: 1);
                        var message = await FollowupAsync($"Select the right emote corresponding to it's name\nEmote Name: {emotes[randomemote]}", components: builder.Build());
                        var choice = await Interactive.NextMessageComponentAsync(x => x.Channel.Id == Context.Channel.Id && x.User.Id == Context.User.Id, timeout: TimeSpan.FromSeconds(10));
                        if (choice.IsSuccess && choice.Value.Data.CustomId == emotes[randomemote])
                        {
                            await message.ModifyAsync(x => { x.Components = new ComponentBuilder().Build(); x.Content = $"Good work! you got payed ${payamount}"; });
                            success = true;
                        }
                        else
                        {
                            await message.ModifyAsync(x => { x.Components = new ComponentBuilder().Build(); x.Content = $"Terrible work! You got payed nothing!"; });
                            success = false;
                        }
                        break;
                    case 2:
                        string[] words = { "fishing", "deposited", "walking", "lights", "computer", "education", "garbage", "windows", "universal", "virtual", "curtains", "heartbeat", "fencing", "minecraft", "furnace", "orange", "animal", "planet", "economy", "profile", "telephone", "bucket", "blanket", "fighting", "graphics", "leather", "string", "station", "diamond", "turning" };
                        string randomword = words[new Random().Next(words.Length)];
                        var message2 = await FollowupAsync($"**Look at the word in the button closely**", components: new ComponentBuilder().WithButton(randomword, "RandomWord").Build());
                        await Task.Delay(5000);
                        await message2.ModifyAsync(x => { x.Components = new ComponentBuilder().Build(); x.Content = "**Now retype the word that was in the button**"; });
                        var choice2 = await Interactive.NextMessageAsync(x => x.Channel.Id == Context.Channel.Id && x.Author.Id == Context.User.Id, timeout: TimeSpan.FromSeconds(10));
                        if (choice2.IsSuccess && choice2.Value.Content.ToLower() == randomword)
                        {
                            await message2.ModifyAsync(x => { x.Components = new ComponentBuilder().Build(); x.Content = $"Good work! you got payed ${payamount}"; });
                            success = true;
                        }
                        else
                        {
                            await message2.ModifyAsync(x => { x.Components = new ComponentBuilder().Build(); x.Content = $"Terrible work! You got payed nothing!"; });
                            success = false;
                        }
                        break;
                }

                if (success)
                {
                    profile.Wallet += payamount;
                    await db.UpdateUserProfile(profile);
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

        [SlashCommand("quitjob", "Quits your current job")]
        public async Task QuitJob()
        {
            try
            {
                if (!await db.UserHasProfile(Context.User.Id))
                {
                    await RespondAsync("Please create an account first! */createaccount*");
                    return;
                }
                var profile = await db.GetUserProfileById(Context.User.Id);
                if (profile.WorkType == "None")
                {
                    await RespondAsync("You don't have a job :thinking:");
                    return;
                }
                profile.WorkType = "None";
                await db.UpdateUserProfile(profile);
                utils.Cooldown(Context.User, "JobHire", 3600);
                await RespondAsync("You have quit your job. You must wait 1 Hour before applying for a new job");
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

        [SlashCommand("jobs", "Shows available jobs depending on your level")]
        public async Task SelectJob()
        {
            try
            {
                if (!await db.UserHasProfile(Context.User.Id))
                {
                    await RespondAsync("Please create an account first! */createaccount*");
                    return;
                }
                var cooldown = utils.Cooldown(Context.User, "JobHire", 3600);
                if (!cooldown.CooledDown)
                {
                    await RespondAsync($"You have recently quit a previous job in the past hour. Please try again in {cooldown.Seconds}");
                    return;
                }
                List<List<string>> jobs = new List<List<string>> { new List<string> { "DiscordMod", "$200" }, new List<string> { "DiscordAdmin", "$400" } };
                var profile = await db.GetUserProfileById(Context.User.Id);
                var embed = new EmbedBuilder().WithTitle("Available Jobs").WithColor(utils.randomColor());
                var menu = new SelectMenuBuilder()
                    .WithPlaceholder("Select a job")
                    .WithCustomId($"SelectJob:{Context.User.Id}");
                int lvlrq = 0;
                foreach (var job in jobs)
                {
                    if (profile.Level >= lvlrq)
                    {
                        menu.AddOption(job[0], job[0], $"Work as a {job[0]}");
                        embed.AddField(job[0], $"{job[1]} per hour");
                    }
                    else
                    {
                        break;
                    }
                    lvlrq += 5;
                }
                var builder = new ComponentBuilder()
                    .WithSelectMenu(menu);
                await RespondAsync(embed: embed.Build(), components: builder.Build());
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

        [SlashCommand("mine", "Mining ores")]
        public async Task Mine()
        {
            try
            {
                await DeferAsync();
                if (!await db.UserHasProfile(Context.User.Id))
                {
                    await FollowupAsync("Please create an account first! */createaccount*");
                    return;
                }
                var cooldown = utils.Cooldown(Context.User, "Mine", 600);
                if (!cooldown.CooledDown)
                {
                    await FollowupAsync($"You are on cooldown for this command! Try again in {cooldown.Seconds} seconds");
                    return;
                }
                var embed = new EmbedBuilder()
                .WithTitle("Price Guide")
                .WithDescription("Stone: $1\nIron: $10\nDiamond: $30\nYou can only mine 4 blocks each mine")
                .WithFooter("Blocks Mined:0")
                .AddField("Money Earned", "$0")
                .WithColor(utils.randomColor());
                var builder = new ComponentBuilder();

                List<string> ores = new List<string> { "stone", "stone", "stone", "stone", "stone", "stone", "stone", "stone", "stone", "stone", "stone", "stone", "stone", "stone", "stone", "iron", "iron", "iron", "iron", "iron", "iron", "iron", "diamond", "diamond", "diamond" };
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        builder.WithButton(customId: $"Mine:{Context.User.Id},{ores[new Random().Next(ores.Count - 1)]},{i},{j}", emote: new Emoji("⬜"), row: i);
                    }
                }
                var msg = await FollowupAsync(embed: embed.Build(), components: builder.Build());
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

        [SlashCommand("pay", "Pay a user")]
        public async Task Pay(SocketGuildUser User, int Amount)
        {
            try
            {
                if (Amount <= 0)
                {
                    await RespondAsync("Amount must higher than $0");
                    return;
                }
                if (User.Id == Context.User.Id)
                {
                    await RespondAsync("You can't pay to yourself dummy");
                    return;
                }
                int rnd = new Random().Next(1, 10);
                if (!await db.UserHasProfile(Context.User.Id))
                {
                    await RespondAsync("Please create an account first! */createaccount*");
                    return;
                }

                if (!await db.UserHasProfile(User.Id))
                {
                    await RespondAsync("This user does not have an account!");
                    return;
                }

                var Payer = await db.GetUserProfileById(Context.User.Id);
                if (Payer.Wallet < Amount)
                {
                    await RespondAsync("You don't have that much money in your wallet!");
                    return;
                }
                var Reciever = await db.GetUserProfileById(User.Id);
                Reciever.Wallet += Amount;
                Payer.Wallet -= Amount;

                await db.UpdateUserProfile(Reciever);
                await db.UpdateUserProfile(Payer);
                await RespondAsync($"Successfully payed {User.Mention}: ${Amount}");
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
