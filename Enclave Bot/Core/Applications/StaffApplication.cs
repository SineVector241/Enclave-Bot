using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;

namespace Enclave_Bot.Core.Applications
{
    public class StaffApplication : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        public InteractiveService Interactive { get; set; }
        private MessageComponent BuildApplicationComponents(string? guildId = null)
        {
            var builder = new ComponentBuilder();
            if (guildId == null)
                guildId = Context.Guild.Id.ToString();

            var selection = new SelectMenuBuilder()
                .WithPlaceholder("Answer Question")
                .WithCustomId($"ASAQ:{guildId}");
            var questions = Database.Settings.Current.ApplicationSettings.StaffApplicationQuestions;

            for (int i = 0; i < questions.Count; i++)
                selection.AddOption($"Answer Question: {i + 1}", i.ToString(), "Answers the question");

            builder.WithSelectMenu(selection);
            builder.WithButton("Submit", $"SSA:{guildId}", ButtonStyle.Success, new Emoji("✅"));

            return builder.Build();
        }

        //DM's the staff application questions
        [ComponentInteraction("SSAQ")]
        public async Task SendStaffApplicationQuestions()
        {
            var user = (SocketGuildUser)Context.User;
            if (user.Roles.FirstOrDefault(x => x.Id == Database.Settings.Current.RoleSettings.VerifiedRole) == null)
            {
                await RespondAsync("Error: You must verify first before filling out a staff application.", ephemeral: true);
                return;
            }
            var embed = new EmbedBuilder()
                .WithTitle("Staff Application Questions")
                .WithDescription("Answer the following questions below.\nTo answer the following questions" +
                " click on the selection menu and select an option to answer the quesiton.")
                .WithAuthor(Context.Interaction.User)
                .WithColor(Utils.RandomColor());

            var questions = Database.Settings.Current.ApplicationSettings.StaffApplicationQuestions;
            if (questions.Count == 0)
            {
                await RespondAsync("Error: Staff Application Questions could not generate as there are no questions.", ephemeral: true);
                return;
            }

            for (int i = 0; i < questions.Count; i++)
                embed.AddField($"Question {i + 1}: {questions[i]}", "Unanswered");

            await DeferAsync();
            try
            {
                var msg = await Context.Interaction.User.SendMessageAsync(embed: embed.Build(), components: BuildApplicationComponents());
                await FollowupAsync(embed: new EmbedBuilder().WithTitle("Staff Application Sent").WithDescription($"Please check your DM's to fill out the application\n[Or Click Here]({msg.GetJumpUrl()}) to jump to the message.").WithColor(Color.Green).Build(), ephemeral: true);
            }
            catch
            (Exception ex)
            {
                await FollowupAsync("Error. Cannot send staff application. Please make sure your DM's are opened and try again.", ephemeral: true);
                Console.WriteLine(ex);
                return;
            }
        }

        //Answer Application Question
        [ComponentInteraction("ASAQ:*")]
        public async Task AnswerStaffApplicationQuestion(string guildId, string[] selection)
        {
            var embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.First());
            var questions = Database.Settings.Current.ApplicationSettings.StaffApplicationQuestions;
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

        [ComponentInteraction("SSA:*")]
        public async Task SubmitStaffApplication(string GuildId)
        {
            var channelId = Database.Settings.Current.ApplicationSettings.StaffApplicationChannel;
            var channel = Context.Client.GetGuild(ulong.Parse(GuildId)).GetTextChannel(channelId);
            if (channel == null)
            {
                await RespondAsync("Error: Could not get submission channel. Cannot Send Staff Application");
                return;
            }

            var builder = new ComponentBuilder()
                .WithButton("Accept", $"ASA:{Context.User.Id}", ButtonStyle.Success, new Emoji("✅"))
                .WithButton("Deny", $"SAD:{Context.User.Id}", ButtonStyle.Secondary, new Emoji("❌"))
                .Build();

            await channel.SendMessageAsync(embed: Context.Interaction.Message.Embeds.FirstOrDefault(), components: builder);
            await Context.Interaction.Message.DeleteAsync();
            await RespondAsync("Successfully Sent Application");
        }

        //Accept Application
        [RequireUserPermission(GuildPermission.Administrator)]
        [ComponentInteraction("ASA:*")]
        public async Task AcceptStaffApplication(string UserId)
        {
            if(Database.Settings.Current.RoleSettings.StaffAcceptRoles.Count <= 0)
            {
                await RespondAsync("Error. Cannot build acceptance roles", ephemeral: true);
                return;
            }

            await DeferAsync();

            var staffAcceptanceRoles = new SelectMenuBuilder()
                .WithCustomId($"SACR:{UserId}")
                .WithPlaceholder("Choose Role");

            for(int i = 0; i < Database.Settings.Current.RoleSettings.StaffAcceptRoles.Count; i++)
            {
                var role = Context.Guild.GetRole(Database.Settings.Current.RoleSettings.StaffAcceptRoles[i]);
                staffAcceptanceRoles.AddOption(role.Name, role.Id.ToString());
            }
            await Context.Interaction.Message.ModifyAsync(x => {
                x.Components = new ComponentBuilder().WithSelectMenu(staffAcceptanceRoles).Build();
                x.Content = $"Staff Application Accept: Staff Accepting - {Context.User.Mention}";
            });
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [ComponentInteraction("SACR:*")]
        public async Task StaffAcceptChooseRole(string UserId)
        {
            var guildUsers = await Context.Guild.GetUsersAsync().FlattenAsync();
            var guildUser = guildUsers.FirstOrDefault(x => x.Id == ulong.Parse(UserId));

            var settings = Database.Settings.Current.RoleSettings;

            var staffRole = Context.Guild.GetRole(settings.StaffRole);
            if (guildUser == null)
            {
                await RespondAsync("Could not find user as the user may have left the server. Cannot Accept Application - Deleted Application", ephemeral: true);
                await Context.Interaction.Message.DeleteAsync();
                return;
            }
            if (staffRole == null)
            {
                await RespondAsync("Could not find required staff role. Cannot Accept Application", ephemeral: true);
                return;
            }

            await DeferAsync();
            await guildUser.AddRoleAsync(staffRole);
            await Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Content = $"Staff Application Accepted By: {Context.Interaction.User.Mention}";
                x.Components = null;
            });

            try
            {
                await guildUser.SendMessageAsync($"Congratulations. Your staff application has been accepted! Welcome to {Context.Guild.Name} staff team!");
            }
            catch { }

            await FollowupAsync("Accepted Staff Application", ephemeral: true);
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [ComponentInteraction("SAD:*")]
        public async Task StaffApplicationDeny(string UserId)
        {
            await DeferAsync();
            var guildUser = Context.Guild.GetUser(ulong.Parse(UserId));
            var embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.FirstOrDefault());

            if (guildUser == null)
            {
                await RespondAsync("Could not find user as the user may have left the server. Cannot Accept Staff Application - Deleted Staff Application", ephemeral: true);
                await Context.Interaction.Message.DeleteAsync();
                return;
            }

            await FollowupAsync("Please provide a reason for the denial...", ephemeral: true);
            var reason = await Interactive.NextMessageAsync(x => x.Author.Id == Context.User.Id && x.Channel.Id == Context.Interaction.ChannelId, timeout: TimeSpan.FromSeconds(5));

            if (reason.IsSuccess)
            {
                await Context.Interaction.Message.ModifyAsync(x =>
                {
                    x.Content = $"Staff Application Denied By: {Context.User.Mention}\nReason: {reason.Value.Content}";
                    x.Embed = embed.Build();
                    x.Components = null;
                });
                await reason.Value.DeleteAsync();

                try
                {
                    await guildUser.SendMessageAsync(embed: new EmbedBuilder()
                        .WithTitle("Staff Application Denied").WithDescription("Your staff application has unfortunately been denied.")
                        .AddField("Reason", reason.Value.Content)
                        .WithColor(Color.Red).Build());
                }
                catch
                { }
            }
            else
            {
                await FollowupAsync("Error. Timed Out", ephemeral: true);
            }
        }
    }
}
