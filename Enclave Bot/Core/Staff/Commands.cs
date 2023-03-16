using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Enclave_Bot.Core.Staff
{
    [StaffOnly]
    [Group("staff", "Staff only commands.")]
    public class Commands : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("purge", "Clears a certain amount of messages in a channel")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Purge(int Amount)
        {
            try
            {
                await DeferAsync();
                SocketTextChannel channel = Context.Channel as SocketTextChannel;
                var msgs = await channel.GetMessagesAsync(limit: Amount + 1).FlattenAsync();
                await channel.DeleteMessagesAsync(msgs);
                await Context.Channel.SendMessageAsync($"Successfully cleared {Amount} messages");
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

        [SlashCommand("kick", "Kicks a user from the guild")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser User, string Reason = "No Reason")
        {
            try
            {
                await User.KickAsync(Reason);
                await RespondAsync($"**Kicked User:** {User.Username} \n**Reason:** {Reason}");
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

        [SlashCommand("ban", "Bans a user from the guild")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser User, string Reason = "No Reason")
        {
            try
            {
                await User.BanAsync(1, Reason);
                await RespondAsync($"**Banned User:** {User.Username} \n**Reason:** {Reason}");
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

        [SlashCommand("embed", "Sends a quick embed message")]
        public async Task Embed(SocketTextChannel channel,string title, string? description = null, [Choice("Application", "Application")] string? withComponents = null)
        {
            var embed = new EmbedBuilder().WithTitle(title).WithDescription(description).WithColor(Utils.RandomColor());
            var builder = new ComponentBuilder();

            switch(withComponents)
            {
                case "Application":
                    builder.WithButton("Application", "SAQ");
                    builder.WithButton("Staff Application", "SSAQ");
                    break;
            }

            if (withComponents == null)
                await channel.SendMessageAsync(embed: embed.Build());
            else
                await channel.SendMessageAsync(embed: embed.Build(), components: builder.Build());
            await RespondAsync("Sent Embed", ephemeral: true);
        }

        [SlashCommand("user-info", "Displays information about a user")]
        public async Task UserInfo(SocketGuildUser? user = null)
        {
            if (user == null)
                user = Context.User as SocketGuildUser;

            var userDb = Database.Users.GetUserById(user.Id);

            var embed = new EmbedBuilder()
                .WithTitle($"UserInfo: {user.DisplayName}")
                .WithThumbnailUrl(user.GetAvatarUrl())
                .AddField("User Id", user.Id)
                .AddField("Status", user.Status)
                .AddField("Created", user.CreatedAt)
                .AddField("Joined", user.JoinedAt)
                .AddField("Last Seen Active", userDb.LastActiveUnix)
                .AddField("Set Gamertag", userDb.Gamertag)
                .WithColor(Utils.RandomColor())
                .Build();

            await RespondAsync(embed: embed);
        }

        [SlashCommand("poll", "Create a poll. Use | to separate answers")]
        public async Task Poll(string question, string options)
        {
            await DeferAsync();

            string[] regs = { "🇦", "🇧", "🇨", "🇩", "🇪", "🇫", "🇬", "🇭", "🇮", "🇯", "🇰", "🇱", "🇲", "🇳", "🇴", "🇵", "🇶", "🇷", "🇸", "🇹", "🇺", "🇻", "🇼", "🇽", "🇾", "🇿" };
            string[] listOptions = options.Split("|");
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor())
                .WithTitle(question);
            string content = "";
            for (int i = 0; i < listOptions.Length; i++)
            {
                content += $"{regs[i]}**: {listOptions[i]}**\n";
            }
            embed.WithDescription(content);

            var msg = await FollowupAsync(embed: embed.Build());
            for (int i = 0; i < listOptions.Length; i++)
            {
                await msg.AddReactionAsync(new Emoji(regs[i]));
            }
        }
    }
}
