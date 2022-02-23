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
                if((ulong)Convert.ToInt64(UserID) != Context.User.Id)
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
                if(profile.WorkType != "None")
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
    }
}
