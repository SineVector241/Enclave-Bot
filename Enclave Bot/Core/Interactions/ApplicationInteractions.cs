using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;

namespace Enclave_Bot.Core.Interactions
{
    public class ApplicationInteractions : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        public InteractiveService Interactive { get; set; }
        private Database.Database db = new Database.Database();

        [ComponentInteraction("SelectAppQuestion")]
        public async Task SelectApplicationQuestion(string[] output)
        {
            try
            {
                await DeferAsync();
                var embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.First());
                var embedFields = embed.Fields;
                int selected = Convert.ToInt16(output.First()) - 1;
                var q = await ReplyAsync($"Question {embedFields[selected].Name}");
                var answer = await Interactive.NextMessageAsync(x => x.Channel is SocketDMChannel && x.Author.Id == Context.User.Id, timeout: TimeSpan.FromMinutes(2));
                if (answer.Value == null)
                    throw new Exception("Timed out");
                embedFields[selected].Value = answer.Value.Content;
                await Context.Interaction.Message.ModifyAsync(x => x.Embed = embed.Build());
                await q.DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var embed = new EmbedBuilder()
                    .WithTitle("An error has occured")
                    .WithDescription($"Error Message: {ex.Message}")
                    .WithColor(Color.DarkRed);
                await ReplyAsync(embed: embed.Build());
            }
        }

        [ComponentInteraction("SubmitApp:*")]
        public async Task SubmitApplication(string guildID)
        {
            try
            {
                var embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.First());
                var guild = Context.Client.GetGuild((ulong)Convert.ToInt64(guildID));
                var guildSettings = await db.GetGuildSettingsById(guild.Id);
                var applicationChannel = guild.GetTextChannel(guildSettings.ApplicationChannel);
                var builder = new ComponentBuilder()
                    .WithButton("Accept", $"ApplicationAccept:{Context.User.Id}", ButtonStyle.Success, new Emoji("✅"))
                    .WithButton("Deny", $"ApplicationDeny:{Context.User.Id}", ButtonStyle.Secondary, new Emoji("❌"))
                    .WithButton("Kick", $"KickDeny:{Context.User.Id}", ButtonStyle.Danger, new Emoji("❕"));
                embed.Title = $"New Application from {Context.User.Username}";
                await applicationChannel.SendMessageAsync($"New application from {Context.User.Mention}", embed: embed.Build(), components: builder.Build());
                await RespondAsync("Successfully sent application ✅");
                await Context.Interaction.Message.DeleteAsync();
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

        [ComponentInteraction("ApplicationAccept:*")]
        public async Task ApplicationAccept(string userID)
        {
            try
            {
                if (Context.Guild != null)
                {
                    var user = Context.Guild.GetUser((ulong)Convert.ToInt64(userID));
                    var settings = await db.GetGuildSettingsById(Context.Guild.Id);
                    await user.AddRoleAsync(settings.VerifiedRole);
                    await user.RemoveRoleAsync(settings.UnverifiedRole);
                    await Context.Interaction.Message.ModifyAsync(x => { x.Content = $"Accepted User {user.Mention}. Accepted by {Context.User.Mention}"; x.Components = new ComponentBuilder().Build(); });
                    await user.SendMessageAsync(embed: new EmbedBuilder().WithTitle("You have been accepted").WithDescription("Your application has been accepted! Welcome to Enclave Kingdoms!").WithColor(Color.Green).Build());
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

        [ComponentInteraction("ApplicationDeny:*")]
        public async Task ApplcationDeny(string userID)
        {
            try
            {
                var embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.First());
                var menu = new SelectMenuBuilder()
                    .WithMinValues(0)
                    .WithMaxValues(embed.Fields.Count)
                    .WithCustomId($"DenyQuestions:{userID}");
                for (int i = 0; i < embed.Fields.Count; i++)
                {
                    menu.AddOption($"Question: {i + 1}", $"{i + 1}");
                }
                menu.AddOption("Deny Application", "Deny", "Denies this application", new Emoji("❌"));
                var builder = new ComponentBuilder()
                    .WithSelectMenu(menu, row: 0);
                await DeferAsync();
                await Context.Interaction.Message.ModifyAsync(x => { x.Components = builder.Build(); });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var embed = new EmbedBuilder()
                    .WithTitle("An error has occured")
                    .WithDescription($"Error Message: {ex.Message}")
                    .WithColor(Color.DarkRed);
                await ReplyAsync(embed: embed.Build());
            }
        }

        [ComponentInteraction("DenyQuestions:*")]
        public async Task DenyQuestions(string userID, string[] output)
        {
            try
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
                var embedApp = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.First()).WithTitle("Enclave Verification Application");
                var user = Context.Guild.GetUser((ulong)Convert.ToInt64(userID));
                var embed = new EmbedBuilder()
                    .WithTitle("You application has been denied")
                    .WithColor(Color.Red);
                string selections = "";
                foreach(var selection in output)
                {
                    if(selection == "Deny")
                    {
                        await DeferAsync();
                        embed.Description = $"Questions that were denied in your application:\n{selections}";
                        var selectMenu = new SelectMenuBuilder()
                            .WithPlaceholder("Select a question")
                            .WithCustomId("SelectAppQuestion");
                        for (int i = 0; i < questions.Length; i++)
                        {
                            selectMenu.AddOption($"Question: {i + 1}", $"{i + 1}", $"Answer question {i + 1}");
                        }
                        var builder = new ComponentBuilder()
                            .WithSelectMenu(selectMenu, row: 0)
                            .WithButton("Submit", $"SubmitApp:{Context.Guild.Id}", ButtonStyle.Success, new Emoji("✅"), row: 1);
                        await user.SendMessageAsync(embed: embed.Build());
                        await user.SendMessageAsync(embed: embedApp.Build(), components: builder.Build());
                        await Context.Interaction.Message.ModifyAsync(x => { x.Content = $"Denied User {user.Mention}. Denied by {Context.User.Mention}"; x.Components = new ComponentBuilder().Build(); });
                        return;
                    }
                    selections += $"Question: {selection}\n";
                }
                await RespondAsync($"Selected questions:\n{selections}", ephemeral:true);
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

        [ComponentInteraction("KickDeny:*")]
        public async Task KickDeny(string userID)
        {
            try
            {
                var user = Context.Guild.GetUser((ulong)Convert.ToInt64(userID));
                await user.SendMessageAsync("Your application has been denied too many times and you were kicked out of the server");
                await user.KickAsync("Application Denied Too Many Times");
                await Context.Interaction.Message.ModifyAsync(x => { x.Content = $"Kicked and denied user {user.Username}. Denied by {Context.User.Mention}"; x.Components = new ComponentBuilder().Build(); });
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

        [ComponentInteraction("SelectStaffAppQuestion")]
        public async Task SelectStaffApplicationQuestion(string[] output)
        {
            try
            {
                await DeferAsync();
                var embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.First());
                var embedFields = embed.Fields;
                int selected = Convert.ToInt16(output.First()) - 1;
                var q = await ReplyAsync($"Question {embedFields[selected].Name}");
                var answer = await Interactive.NextMessageAsync(x => x.Channel is SocketDMChannel && x.Author.Id == Context.User.Id, timeout: TimeSpan.FromMinutes(2));
                if (answer.Value == null)
                    throw new Exception("Timed out");
                embedFields[selected].Value = answer.Value.Content;
                await Context.Interaction.Message.ModifyAsync(x => x.Embed = embed.Build());
                await q.DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                var embed = new EmbedBuilder()
                    .WithTitle("An error has occured")
                    .WithDescription($"Error Message: {ex.Message}")
                    .WithColor(Color.DarkRed);
                await ReplyAsync(embed: embed.Build());
            }
        }

        [ComponentInteraction("SubmitStaffApp:*")]
        public async Task SubmitStaffApplication(string guildID)
        {
            try
            {
                var embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.First());
                var guild = Context.Client.GetGuild((ulong)Convert.ToInt64(guildID));
                var guildSettings = await db.GetGuildSettingsById(guild.Id);
                var applicationChannel = guild.GetTextChannel(guildSettings.StaffApplicationChannel);
                var builder = new ComponentBuilder()
                    .WithButton("Accept", $"StaffApplicationAccept:{Context.User.Id}", ButtonStyle.Success, new Emoji("✅"))
                    .WithButton("Deny", $"StaffApplicationDeny:{Context.User.Id}", ButtonStyle.Danger, new Emoji("❌"));
                embed.Title = $"New Application from {Context.User.Username}";
                await applicationChannel.SendMessageAsync($"New staff application from {Context.User.Mention}", embed: embed.Build(), components: builder.Build());
                await RespondAsync("Successfully sent staff application ✅");
                await Context.Interaction.Message.DeleteAsync();
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

        [ComponentInteraction("StaffApplicationAccept:*")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task StaffApplicationAccept(string userID)
        {
            try
            {
                await DeferAsync();
                var builder = new ComponentBuilder();
                var menu = new SelectMenuBuilder()
                    .WithCustomId($"SelectStaffRole:{userID}")
                    .AddOption("Baron", "757462916613144616")
                    .AddOption("Marchion", "757463457510457396")
                    .AddOption("Viscount", "757463208855339078");
                builder.WithSelectMenu(menu);
                await Context.Interaction.Message.ModifyAsync(x => { x.Components = builder.Build(); x.Content = $"Accepting user <@{userID}>: Staff Accepting: {Context.Interaction.User.Mention}"; });
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

        [ComponentInteraction("SelectStaffRole:*")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SelectStaffRole(string userID)
        {

        }
    }
}