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
            await DeferAsync();
            if (userID != Context.User.Id.ToString())
            {
                return;
            }
            Emote emoji = null;
            switch (Ore)
            {
                case "stone":
                    emoji = Emote.Parse("<:stone:946379258979295253>");
                    break;
                case "iron":
                    emoji = Emote.Parse("<:iron:923831992544555069>");
                    break;
                case "diamond":
                    emoji = Emote.Parse("<:diamond:946380040894021632>");
                    break;
            }
            var builder2 = new ComponentBuilder();
            var olderbuilder = ComponentBuilder.FromMessage(Context.Interaction.Message);
            var rows = olderbuilder.ActionRows;
            for (int j = 0; j < rows.Count; j++)
            {
                int counter = 0;
                foreach (var component in rows[j].Components)
                {
                    switch (component)
                    {
                        case ButtonComponent button:
                            if (button.IsDisabled)
                            {
                                counter++;
                            }
                            break;
                    }
                    if (counter == 4)
                        await Context.Interaction.Message.ModifyAsync(x => x.Components = new ComponentBuilder().Build());
                }
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
            await Context.Interaction.Message.ModifyAsync(x => { x.Components = builder2.Build(); });
        }
    }
}
