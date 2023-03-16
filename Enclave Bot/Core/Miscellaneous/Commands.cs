using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Enclave_Bot.Core.Miscellaneous
{
    public class Commands : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("setgamertag", "Sets your gamertag inside the bot.")]
        public async Task SetGamertag(string gamertag)
        {
            var user = Database.Users.GetUserById(Context.User.Id);
            user.Gamertag = gamertag;
            Database.Users.UpdateUser(user);
            await RespondAsync("Successfully updated your gamertag", ephemeral: true);
        }

        [SlashCommand("suggest", "Sends a suggestion")]
        public async Task Suggest(string suggestion)
        {
            var setting = Database.Settings.Current.MiscellaneousSettings.SuggestionsChannel;
            var channel = Context.Guild.GetTextChannel(setting);
            var embed = new EmbedBuilder()
                .WithTitle("New Suggestion")
                .WithDescription(suggestion)
                .WithColor(Utils.RandomColor())
                .WithAuthor(Context.User)
                .Build();

            if(channel == null)
            {
                await RespondAsync("Error. Could not send suggestion as suggestion channel is not set", ephemeral: true);
                return;
            }

            await DeferAsync(ephemeral: true);
            var msg = await channel.SendMessageAsync(embed: embed);
            await msg.AddReactionsAsync(new List<Emoji>() { new Emoji("✅"), new Emoji("❌") });
            await FollowupAsync("Sent Suggestion!", ephemeral: true);
        }
    }
}
