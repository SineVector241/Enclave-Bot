using Discord.Interactions;
using Discord.WebSocket;
using Discord;

namespace Enclave_Bot.Core.Interactions
{
    public class EconomyInteractions : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        Database.Database db = new Database.Database();

        [ComponentInteraction("SelectJob:*")]
        public async Task SelectJob(string UserID, string[] output)
        {
            try
            {
                if ((ulong)Convert.ToInt64(UserID) != Context.User.Id)
                {
                    await RespondAsync("This is not your interaction page!");
                    return;
                }
                if (!await db.UserHasProfile(Context.User.Id))
                {
                    await RespondAsync("Please create an account first! */createaccount*");
                    return;
                }

                var profile = await db.GetUserProfileById(Context.User.Id);
                if (profile.WorkType != "None")
                {
                    await RespondAsync("You already have a job. Please leave the job before selecting a new one!");
                    return;
                }
                profile.WorkType = output.First();
                await db.UpdateUserProfile(profile);
                await RespondAsync($"You now work as a {output.First()}");
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

        [ComponentInteraction("Mine:*,*,*,*")]
        public async Task MineInteraction(string userID, string Ore, string row, string comp)
        {
            try
            {
                int blocksmined = Convert.ToInt16(Context.Interaction.Message.Embeds.First().Footer.Value.Text.Replace("Blocks Mined:", ""));
                var embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.First()).WithFooter($"Blocks Mined:{blocksmined += 1}");
                int Money = Convert.ToInt16(embed.Fields.First().Value.ToString().Replace("$", ""));
                if (userID != Context.User.Id.ToString())
                {
                    await RespondAsync("This is not your mine!", ephemeral: true);
                    return;
                }
                Emote emoji = null;
                switch (Ore)
                {
                    case "stone":
                        Money += 1;
                        emoji = Emote.Parse("<:stone:946379258979295253>");
                        break;
                    case "iron":
                        Money += 10;
                        emoji = Emote.Parse("<:iron:923831992544555069>");
                        break;
                    case "diamond":
                        Money += 30;
                        emoji = Emote.Parse("<:diamond:946380040894021632>");
                        break;
                }
                embed.Fields.First().Value = $"${Money}";
                if (blocksmined == 4)
                {
                    var userprofile = await db.GetUserProfileById(Context.User.Id);
                    userprofile.Wallet += Money;
                    await db.UpdateUserProfile(userprofile);
                    await Context.Interaction.Message.ModifyAsync(x => { x.Components = new ComponentBuilder().Build(); x.Embed = embed.Build(); });
                    await DeferAsync();
                    return;
                }
                var builder2 = new ComponentBuilder();
                var olderbuilder = ComponentBuilder.FromMessage(Context.Interaction.Message);
                var rows = olderbuilder.ActionRows;
                for (int j = 0; j < rows.Count; j++)
                {
                    foreach (var component in rows[j].Components)
                    {
                        switch (component)
                        {
                            case ButtonComponent button:
                                if (button.CustomId == Context.Interaction.Data.CustomId)
                                {
                                    builder2.WithButton(button.ToBuilder()
                                        .WithDisabled(true).WithEmote(emoji), j);
                                }
                                else
                                {
                                    builder2.WithButton(button.ToBuilder(), j);
                                }
                                break;
                        }
                    }
                }
                await Context.Interaction.Message.ModifyAsync(x => { x.Components = builder2.Build(); x.Embed = embed.Build(); });
                await DeferAsync();
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
