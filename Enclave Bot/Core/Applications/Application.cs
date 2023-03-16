using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;

namespace Enclave_Bot.Core.Applications
{
    public class Application : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        public InteractiveService Interactive { get; set; }
        private static List<CADValues> CADSelections = new List<CADValues>();
        private MessageComponent BuildApplicationComponents(string? guildId = null)
        {
            var builder = new ComponentBuilder();
            if (guildId == null)
                guildId = Context.Guild.Id.ToString();

            var selection = new SelectMenuBuilder()
                .WithPlaceholder("Answer Question")
                .WithCustomId($"AAQ:{guildId}");
            var questions = Database.Settings.Current.ApplicationSettings.ApplicationQuestions;

            for (int i = 0; i < questions.Count; i++)
                selection.AddOption($"Answer Question: {i + 1}", i.ToString(), "Answers the question");

            builder.WithSelectMenu(selection);
            builder.WithButton("Submit", $"SA:{guildId}", ButtonStyle.Success, new Emoji("✅"));

            return builder.Build();
        }

        //DM's the application questions
        [ComponentInteraction("SAQ")]
        public async Task SendApplicationQuestions()
        {
            var user = Database.Users.GetUserById(Context.User.Id);
            if(string.IsNullOrWhiteSpace(user.Gamertag))
            {
                await RespondAsync("Please set your gamertag first by running the `/setgamertag` command!!", ephemeral: true);
                return;
            }
            var embed = new EmbedBuilder()
                .WithTitle("Application Questions")
                .WithDescription("Answer the following questions below.\nTo answer the following questions" +
                " click on the selection menu and select an option to answer the quesiton.")
                .WithAuthor(Context.Interaction.User)
                .WithFooter($"Attempt: {user.ApplicationDenials}")
                .WithColor(Utils.RandomColor());

            var questions = Database.Settings.Current.ApplicationSettings.ApplicationQuestions;
            if (questions.Count == 0)
            {
                await RespondAsync("Error: Application Questions could not generate as there are no questions.", ephemeral: true);
                return;
            }

            for (int i = 0; i < questions.Count; i++)
                embed.AddField($"Question {i + 1}: {questions[i]}", "Unanswered");

            await DeferAsync();
            try
            {
                var msg = await Context.Interaction.User.SendMessageAsync(embed: embed.Build(), components: BuildApplicationComponents());
                await FollowupAsync(embed: new EmbedBuilder().WithTitle("Application Sent").WithDescription($"Please check your DM's to fill out the application\n[Or Click Here]({msg.GetJumpUrl()}) to jump to the message.").WithColor(Color.Green).Build(), ephemeral: true);
            }
            catch
            (Exception ex)
            {
                await FollowupAsync("Error. Cannot send application. Please make sure your DM's are opened and try again.", ephemeral: true);
                Console.WriteLine(ex);
                return;
            }
        }

        //Answer Application Question
        [ComponentInteraction("AAQ:*")]
        public async Task AnswerApplicationQuestion(string guildId, string[] selection)
        {
            var embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.First());
            var questions = Database.Settings.Current.ApplicationSettings.ApplicationQuestions;
            var i = int.Parse(selection[0]);

            await RespondAsync($"**{questions[i]}**\nAnswer the question by sending a message below.", ephemeral: true);
            var answer = await Interactive.NextMessageAsync(x => x.Author.Id == Context.Interaction.User.Id && x.Channel is SocketDMChannel, timeout: TimeSpan.FromMinutes(5));
            if (answer.IsSuccess && answer.Value != null)
                embed.Fields[i].Value = answer.Value.Content;
            else
            {
                await FollowupAsync("Error: Timed Out.", ephemeral: true);
                return;
            }

            await Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = embed.Build();
                x.Components = BuildApplicationComponents(guildId);
            });
        }

        [ComponentInteraction("SA:*")]
        public async Task SubmitApplication(string GuildId)
        {
            var embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.FirstOrDefault())
                .WithDescription(null)
                .Build();

            for (int i = 0; i < embed.Fields.Length; i++)
            {
                if (embed.Fields[i].Value == "Unanswered")
                {
                    await RespondAsync("You must answer all questions before submitting.", ephemeral: true);
                    return;
                }
            }
            var channelId = Database.Settings.Current.ApplicationSettings.ApplicationChannel;
            var channel = Context.Client.GetGuild(ulong.Parse(GuildId)).GetTextChannel(channelId);
            if (channel == null)
            {
                await RespondAsync("Error: Could not get submission channel. Cannot Send Application");
                return;
            }

            var builder = new ComponentBuilder()
                .WithButton("Accept", $"AA:{Context.User.Id}", ButtonStyle.Success, new Emoji("✅"))
                .WithButton("Deny", $"AD:{Context.User.Id}", ButtonStyle.Secondary, new Emoji("❌"))
                .Build();

            await channel.SendMessageAsync(embed: embed, components: builder);
            await Context.Interaction.Message.DeleteAsync();
            await RespondAsync("Successfully Sent Application");
        }

        //Accept Application
        [StaffOnly]
        [ComponentInteraction("AA:*")]
        public async Task AcceptApplication(string UserId)
        {
            var guildUsers = await Context.Guild.GetUsersAsync().FlattenAsync();
            var guildUser = guildUsers.FirstOrDefault(x => x.Id == ulong.Parse(UserId));
            var settings = Database.Settings.Current.RoleSettings;

            var verifiedRole = Context.Guild.GetRole(settings.VerifiedRole);
            var unverifiedRole = Context.Guild.GetRole(settings.UnverifiedRole);
            if (guildUser == null)
            {
                await RespondAsync("Could not find user as the user may have left the server. Cannot Accept Application - Deleted Application", ephemeral: true);
                await Context.Interaction.Message.DeleteAsync();
                return;
            }
            if (verifiedRole == null || unverifiedRole == null)
            {
                await RespondAsync("Could not find required verified and unverified roles. Cannot Accept Application", ephemeral: true);
                return;
            }

            await DeferAsync();
            await guildUser.RemoveRoleAsync(unverifiedRole);
            await guildUser.AddRoleAsync(verifiedRole);
            await Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Content = $"Application Accepted By: {Context.Interaction.User.Mention}";
                x.Components = null;
            });

            try
            {
                await guildUser.SendMessageAsync($"Congratulations. Your application has been accepted! Welcome to {Context.Guild.Name}!");
            }
            catch { }

            await FollowupAsync("Accepted Application", ephemeral: true);
        }

        [StaffOnly]
        [ComponentInteraction("AD:*")]
        public async Task ApplicationDeny(string UserId)
        {
            await DeferAsync();
            var user = Database.Users.GetUserById(ulong.Parse(UserId));
            user.ApplicationDenials += 1;
            Database.Users.UpdateUser(user);
            var guildUser = Context.Guild.GetUser(ulong.Parse(UserId));
            var embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.FirstOrDefault());

            if (user.ApplicationDenials >= Database.Settings.Current.ApplicationSettings.KickAfterDenials && Database.Settings.Current.ApplicationSettings.KickAfterDenials != 0)
                if (guildUser != null)
                {
                    await guildUser.KickAsync();
                    user.ApplicationDenials = 0;
                    Database.Users.UpdateUser(user);
                    await Context.Interaction.Message.ModifyAsync(x =>
                    {
                        x.Components = null;
                        x.Content = $"Application Denied By: {Context.Interaction.User.Mention}";
                    });
                    try
                    {
                        await guildUser.SendMessageAsync("You have been kicked from the server after exceeding the application denial limit.");
                    }
                    catch
                    { }
                    await FollowupAsync("User has exceeded denial limit. Kicked User", ephemeral: true);
                    return;
                }

            var selection = new SelectMenuBuilder()
                .WithCustomId("SDAQ")
                .WithMinValues(1)
                .WithMaxValues(embed.Fields.Count);
            if (guildUser == null)
            {
                await RespondAsync("Could not find user as the user may have left the server. Cannot Accept Application - Deleted Application", ephemeral: true);
                await Context.Interaction.Message.DeleteAsync();
                return;
            }

            for (int i = 0; i < embed.Fields.Count; i++)
            {
                selection.AddOption($"Question: {i + 1}", i.ToString(), "Selects Denial Question");
            }

            await Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Content = $"Application Denied By: {Context.User.Mention}";
                x.Embed = embed.WithFooter($"Attempt: {user.ApplicationDenials}").Build();
                x.Components = new ComponentBuilder().WithSelectMenu(selection).WithButton("Confirm", $"CAD:{UserId}", ButtonStyle.Success, new Emoji("✅")).Build();
            });
        }

        [StaffOnly]
        [ComponentInteraction("SDAQ")]
        public async Task SelectDenialApplicationQuestion(string[] selections)
        {
            CADSelections.RemoveAll(x => DateTime.UtcNow.Subtract(x.LastUsed).Minutes > 2);

            var values = CADSelections.FirstOrDefault(x => x.UserId == Context.User.Id && x.MsgId == Context.Interaction.Message.Id);
            if (values == null)
            {
                CADSelections.Add(new CADValues() { Selections = selections, MsgId = Context.Interaction.Message.Id, UserId = Context.User.Id });
            }
            else
            {
                for(int i = 0; i < CADSelections.Count; i++)
                {
                    if(CADSelections[i].MsgId == Context.Interaction.Message.Id && CADSelections[i].UserId == Context.User.Id) 
                    {
                        CADSelections[i].Selections = selections;
                        CADSelections[i].LastUsed = DateTime.UtcNow;
                        break;
                    }
                }
            }
            await DeferAsync();
        }

        [StaffOnly]
        [ComponentInteraction("CAD:*")]
        public async Task ConfirmApplicationDenial(string UserId)
        {
            try
            {
                await DeferAsync();
                var embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.FirstOrDefault());
                var selections = CADSelections.FirstOrDefault(x => x.UserId == Context.User.Id && x.MsgId == Context.Interaction.Message.Id);
                var selectionsMessage = "";

                if (selections == null)
                    return;

                for (int i = 0; i < selections.Selections.Length; i++)
                {
                    selectionsMessage += $"Question: {int.Parse(selections.Selections[i]) + 1}\n";
                }

                CADSelections.RemoveAll(x => x.MsgId == Context.Interaction.Message.Id);

                var guildUsers = await Context.Guild.GetUsersAsync().FlattenAsync();
                var guildUser = guildUsers.FirstOrDefault(x => x.Id == ulong.Parse(UserId));
                if (guildUser == null)
                {
                    await FollowupAsync("Could not find user as the user may have left the server. Cannot Accept Application - Deleted Application", ephemeral: true);
                    await Context.Interaction.Message.DeleteAsync();
                    return;
                }

                embed.WithDescription($"Your application has been denied due to the following questions:\n{selectionsMessage}");
                await guildUser.SendMessageAsync(embed: embed.Build(), components: BuildApplicationComponents());
                await Context.Interaction.Message.ModifyAsync(x => { 
                    x.Components = null;
                    x.Content = Context.Interaction.Message.Content;
                });
                await FollowupAsync("Successfully Denied User", ephemeral: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private class CADValues
        {
            public string[] Selections { get; set; } = Array.Empty<string>();
            public ulong MsgId { get; set; }
            public ulong UserId { get; set; }
            public DateTime LastUsed { get; set; } = DateTime.UtcNow;
        }
    }
}
