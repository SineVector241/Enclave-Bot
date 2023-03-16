using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace Enclave_Bot.Core.ReactionRole
{
    [StaffOnly]
    [Group("rr", "Reaction Role Commands: STAFF ONLY")]
    public class Commands : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        [SlashCommand("create", "Initializes a new reaction role creation screen.")]
        public async Task Create(SocketTextChannel channel)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Reaction Role Builder")
                .WithAuthor(Context.User)
                .WithColor(Utils.RandomColor())
                .WithFooter($"Sends RR To: {channel.Name}, Type: Normal, RequiredRole: None")
                .Build();

            var selectMenu = new SelectMenuBuilder()
                .WithCustomId($"RRBT:{Context.User.Id}")
                .WithPlaceholder("Set RR Type")
                .AddOption("Normal", "1", "Adds and removes roles like a normal reaction.")
                .AddOption("1 Role Only", "2", "Adds and removes roles but only allows 1 role to be set.")
                .AddOption("Add & Remove Role", "3", "Adds a role and removes a role if user has it.")
                .AddOption("Add & Remove Required Role", "4", "Adds a role if the user has the required role to remove.")
                .AddOption("Normal & Requires Role", "5", "Adds and removes roles like a normal reaction but requires a role.");

            var builder = new ComponentBuilder()
                .WithButton("Add", $"RRBA:{Context.User.Id}", ButtonStyle.Success, new Emoji("➕"))
                .WithButton("Del", $"RRBD:{Context.User.Id}", ButtonStyle.Danger, new Emoji("➖"))
                .WithButton("Publish", $"RRBP:{Context.User.Id},{channel.Id}", ButtonStyle.Success, new Emoji("✅"))
                .WithButton("Set Title", $"RRBST:{Context.User.Id}", ButtonStyle.Primary, new Emoji("✏"), row: 1)
                .WithButton("Set Description", $"RRBSD:{Context.User.Id}", ButtonStyle.Primary, new Emoji("✏"), row: 1)
                .WithSelectMenu(selectMenu)
                .Build();

            await RespondAsync(embed: embed, components: builder);
        }
    }
}
