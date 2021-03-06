using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Enclave_Bot.Core.SlashCommands
{
    public class DeveloperCommands : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        Utils utils = new Utils();
        Database.Database db = new Database.Database();

        [SlashCommand("checkcooldown", "Checks a cooldown list")]
        public async Task CheckCooldown(string type)
        {
            try
            {
                if (Context.User.Id != 550912080627236874)
                {
                    return;
                }
                var list = utils.CheckCooldownList(type);
                var embed = new EmbedBuilder();
                embed.WithTitle($"Checking Cooldown: {type}");
                embed.WithColor(utils.randomColor());
                foreach (var cooldown in list)
                {
                    embed.AddField($"ID: {cooldown.UserID}", $"DateTime: {cooldown.DateTime}\nSeconds: {cooldown.CooldownSeconds}");
                }
                await RespondAsync(embed: embed.Build());
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

        [SlashCommand("test", "Test Command")]
        public async Task TestCommand()
        {
            try
            {
                await DeferAsync();
                var data = await db.GetMultipleActivityData(Context.Guild.Id);
                foreach(var activity in data)
                {
                    await Context.Channel.SendMessageAsync(activity.RoleID.ToString());
                }
                await FollowupAsync("Done");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
