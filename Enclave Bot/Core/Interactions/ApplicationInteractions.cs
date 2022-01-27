using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;

namespace Enclave_Bot.Core.Interactions
{
    public class ApplicationInteractions : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        public InteractiveService Interactive { get; set; }

        [ComponentInteraction("fapp")]
        public async Task FactionApplication(string[] e)
        {
            if (Context.Channel is SocketDMChannel)
            {
                if (e.First() == "Submit")
                {
                    await Context.Client.GetGuild(749358542145716275).GetTextChannel(757581056470679672).SendMessageAsync($"<@&796252628837335040>: New Application {Context.User.Mention}", embed: Context.Interaction.Message.Embeds.First(), components: new ComponentBuilder().WithButton("Accept", $"FappAccept:{Context.User.Id}", ButtonStyle.Success, new Emoji("✅")).WithButton("Deny", $"FappDeny:{Context.User.Id}", ButtonStyle.Danger, new Emoji("❌")).WithButton("KickDeny", $"KickDeny:{Context.User.Id}", ButtonStyle.Secondary, new Emoji("❌")).Build());
                    await Context.Interaction.Message.DeleteAsync();
                    await Context.User.SendMessageAsync("✅ Successfully Sent Application");
                }
                _ = Task.Run(async () => { await RespondAsync(); });
                EmbedBuilder embed = new EmbedBuilder()
                    .WithTitle(Context.Interaction.Message.Embeds.First().Title)
                    .WithColor((Color)Context.Interaction.Message.Embeds.First().Color);

                foreach (var field in Context.Interaction.Message.Embeds.First().Fields)
                {
                    if (field.Name.Contains(e.First()))
                    {
                        var msg = await Context.Interaction.Message.ReplyAsync(field.Name);
                        var answer = await Interactive.NextMessageAsync(x => x.Channel is SocketDMChannel && x.Author.Id == Context.User.Id, timeout: TimeSpan.FromMinutes(5));
                        embed.AddField(field.Name, answer.IsSuccess == true ? answer.Value.Content : field.Value);
                        await msg.DeleteAsync();
                    }
                    else
                    {
                        embed.AddField(field.Name, field.Value);
                    }
                }
                await Context.Interaction.Message.ModifyAsync(x => x.Embed = embed.Build());
            }
        }

        [ComponentInteraction("FappAccept:*")]
        public async Task FactionApplicationAccept(string id)
        {
            try
            {
                SocketGuildUser user = Context.Guild.GetUser((ulong)Convert.ToInt64(id));
                await user.AddRoleAsync(Context.Guild.GetRole(757614906051919974));
                await user.RemoveRoleAsync(Context.Guild.GetRole(757613578638590013));
                await Context.Interaction.Message.ModifyAsync(x =>
                {
                    x.Components = new ComponentBuilder().Build();
                    x.Content = $"✅ Accepted User {user.Mention}";
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [ComponentInteraction("FappDeny:*")]
        public async Task FactionApplicationDeny(string id)
        {
            try
            {
                ComponentBuilder builder = new ComponentBuilder();
                builder.WithButton("Question 1", "FappSelectDeny:1", ButtonStyle.Secondary);
                builder.WithButton("Question 2", "FappSelectDeny:2", ButtonStyle.Secondary);
                builder.WithButton("Question 3", "FappSelectDeny:3", ButtonStyle.Secondary);
                builder.WithButton("Question 4", "FappSelectDeny:4", ButtonStyle.Secondary);
                builder.WithButton("Question 5", "FappSelectDeny:5", ButtonStyle.Secondary);
                builder.WithButton("Question 6", "FappSelectDeny:6", ButtonStyle.Secondary, row: 1);
                builder.WithButton("Question 7", "FappSelectDeny:7", ButtonStyle.Secondary, row: 1);
                builder.WithButton("Question 8", "FappSelectDeny:8", ButtonStyle.Secondary, row: 1);
                builder.WithButton("Question 9", "FappSelectDeny:9", ButtonStyle.Secondary, row: 1);
                builder.WithButton("Question 10", "FappSelectDeny:10", ButtonStyle.Secondary, row: 1);
                builder.WithButton("Deny Application", $"DenyApplication:{id}", ButtonStyle.Danger, row: 2);
                await Context.Interaction.Message.ModifyAsync(x => x.Components = builder.Build());
                await RespondAsync();
            }
            catch (Exception e)
            {
            }
        }

        [ComponentInteraction("FappSelectDeny:*")]
        public async Task FactionApplicationDenySelect(string id)
        {
            try
            {
                var builder = new ComponentBuilder();
                string DSselect = "";
                foreach (ActionRowComponent row in Context.Interaction.Message.Components)
                {
                    foreach (ButtonComponent buttonComponent in row.Components)
                    {
                        var button = buttonComponent.ToBuilder();
                        if (button.CustomId == $"FappSelectDeny:{id}" && button.Style is ButtonStyle.Secondary)
                        {
                            button.Style = ButtonStyle.Primary;
                            DSselect = "Selected";
                        }

                        else if (button.CustomId == $"FappSelectDeny:{id}" && button.Style is ButtonStyle.Primary)
                        {
                            button.Style = ButtonStyle.Secondary;
                            DSselect = "Deselected";
                        }
                        builder.WithButton(button);
                    }
                }
                await Context.Interaction.Message.ModifyAsync(x => x.Components = builder.Build());
                await RespondAsync($"{DSselect} Question {id}", ephemeral: true);
            }
            catch (Exception e)
            {
            }
        }

        [ComponentInteraction("DenyApplication:*")]
        public async Task DenyApplication(string id)
        {
            try
            {
                var selectMenu = new SelectMenuBuilder()
                    .WithCustomId("fapp")
                    .WithMaxValues(1)
                    .WithMinValues(1)
                    .WithPlaceholder("Select a question");
                var builder = new ComponentBuilder();
                var embed = Context.Interaction.Message.Embeds.First();
                var embedBuilder = new EmbedBuilder()
                    .WithTitle("Your application has been denied")
                    .WithColor(Color.Red);
                SocketGuildUser user = Context.Guild.GetUser((ulong)Convert.ToInt64(id));
                string deniedquestions = "";
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

                foreach (ActionRowComponent row in Context.Interaction.Message.Components)
                {
                    foreach (ButtonComponent buttonComponent in row.Components)
                    {
                        var button = buttonComponent.ToBuilder();
                        if (button.Style is ButtonStyle.Primary)
                        {
                            deniedquestions += $"\n{button.Label}";
                        }
                    }
                }

                for (int i = 1; i <= questions.Length; i++) selectMenu.AddOption($"Question {i}", $"{i}.", $"Fillout Question {i}");

                selectMenu.AddOption("Submit", "Submit", "Submits the application", new Emoji("✅"));
                builder.WithSelectMenu(selectMenu, row: 0);
                embedBuilder.WithDescription($"The following questions that denied your application were: {deniedquestions}");

                await user.SendMessageAsync(embed: embedBuilder.Build());
                await user.SendMessageAsync(embed: embed, components: builder.Build());
                await Context.Interaction.Message.ModifyAsync(x => { x.Components = new ComponentBuilder().Build(); x.Content = $"Denied Application {user.Mention}"; });
                await RespondAsync();
            }
            catch
            {

            }
        }

        [ComponentInteraction("KickDeny:*")]
        public async Task KickDeny(string id)
        {
            try
            {
                SocketGuildUser user = Context.Guild.GetUser((ulong)Convert.ToInt64(id));
                try
                {
                    await user.SendMessageAsync("Your application has been denied and you have been kicked from the server for failing the application too many times");
                }
                catch
                { }
                await user.KickAsync();
                await Context.Interaction.Message.ModifyAsync(x => { x.Components = new ComponentBuilder().Build(); x.Content = $"Denied Application {user.Mention}"; });
                await RespondAsync();
            }
            catch (Exception ex)
            {
            }
        }
    }
}