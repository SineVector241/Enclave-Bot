using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Enclave_Bot.Core.Miscellaneous
{
    public class Commands : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("bindgamertag", "Binds your gamertag inside the bot. Uses the minecraft server to bind.")]
        public async Task BindGamertag()
        {
            if(HttpServer.ServerCodes.ContainsKey(Context.User.Id))
            {
                HttpServer.ServerCodes.TryGetValue(Context.User.Id, out var value);
                if(value != null)
                {
                    var embedded = new EmbedBuilder()
                        .WithTitle("Bind Code Created")
                        .AddField("Code", value.Code)
                        .AddField("Expires in", $"<t:{Utils.ToUnixTimestamp(value.CreatedAt.AddMinutes(5))}:R>")
                        .WithColor(Color.Green)
                        .Build();
                    await RespondAsync(embed: embedded, ephemeral: false);
                    return;
                }
            }

            var code = new ServerCode()
            {
                Code = new Random().Next(0, 9999)
            };
            HttpServer.ServerCodes.Add(Context.User.Id, code);

            var embed = new EmbedBuilder()
                .WithTitle("Bind Code Created")
                .AddField("Code", code.Code)
                .AddField("Expires in", $"<t:{Utils.ToUnixTimestamp(DateTime.UtcNow.AddMinutes(5))}:R>")
                .WithColor(Color.Green)
                .Build();
            await RespondAsync(embed: embed, ephemeral: false);
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
