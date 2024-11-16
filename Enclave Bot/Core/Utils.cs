using Discord;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;

namespace Enclave_Bot.Core
{
    public class Utils(DatabaseContext database)
    {
        private const int ListLimit = 25;
        private readonly DatabaseContext Database = database;

        public Embed CreateErrorEmbed(string errorDescription, IUser author)
        {
            return new EmbedBuilder()
                    .WithTitle("Error!")
                    .WithDescription($"**Error Message:** {errorDescription}")
                    .WithAuthor(author)
                    .WithColor(Color.Red).Build();
        }

        //Application Stuff
        public ComponentBuilder CreateApplicationEditorComponents(Application application, SocketUser author, int page = 0)
        {
            var appQuestions = Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).Count();

            var components = new ComponentBuilder()
                .WithButton("Add Question", $"AAQ:{application.Id},{author.Id}", ButtonStyle.Success, new Emoji("➕"))
                .WithButton("Remove Question", $"RAQ:{application.Id},{author.Id}", ButtonStyle.Danger, new Emoji("➖"))
                .WithButton("Edit Question", $"EAQ:{application.Id},{author.Id}", ButtonStyle.Primary, new Emoji("✏️"))
                .WithButton("OnSubmit Action", $"OSA:{application.Id},{author.Id}", ButtonStyle.Primary, new Emoji("🔧"), row: 1)
                .WithButton("OnAccept Action", $"OAA:{application.Id},{author.Id}", ButtonStyle.Primary, new Emoji("🔧"), row: 1)
                .WithButton("OnDeny Action", $"ODA:{application.Id},{author.Id}", ButtonStyle.Primary, new Emoji("🔧"), row: 1);

            if (appQuestions > (page + 1) * 25)
                components.WithButton("Next Page", $"GNP:{application.Id},{author.Id},{page + 1}", ButtonStyle.Primary, new Emoji("▶️"), row: 1);
            if (page > 0)
                components.WithButton("Previous Page", $"GPP:{application.Id},{author.Id},{page - 1}", ButtonStyle.Primary, new Emoji("◀️"), row: 1);

            return components;
        }

        public EmbedBuilder CreateApplicationListEmbed(SocketGuild guild, Application[] applications, int page = 0)
        {
            var title = $"{guild.Name} Applications";
            var embed = new EmbedBuilder()
                            .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                            .WithColor(Bot.PrimaryColor);
            //25 for list limit!
            for(var i = page * ListLimit; i < applications.Length && i < (page * ListLimit + ListLimit); i++)
            {
                embed.AddField(applications[i].Name, $"`{applications[i].Id}`");
            }

            return embed;
        }

        public EmbedBuilder CreateApplicationEditorEmbed(Application application, SocketUser author, int page = 0)
        {
            var applicationQuestions = Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).ToArray();
            var title = $"Editing Application {application.Name}";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                .WithColor(Bot.PrimaryColor)
            .WithAuthor(author);

            for (int i = page * ListLimit; i < application.Questions.Count && i < (page * ListLimit + ListLimit); i++)
            {
                embed.AddField(applicationQuestions[i].Index.ToString(), applicationQuestions[i].Question);
            }

            return embed;
        }

        //Actions Stuff
        public ComponentBuilder CreateServerActionBehaviorsEditorComponents(ServerAction action, SocketUser author)
        {
            var components = new ComponentBuilder()
                .WithButton("Add Behavior", $"ASAB:{action.Id},{author.Id}", ButtonStyle.Success, new Emoji("➕"))
                .WithButton("Remove Behavior", $"RSAB:{action.Id},{author.Id}", ButtonStyle.Danger, new Emoji("➖"))
                .WithButton("Edit Behavior", $"ESAB:{action.Id},{author.Id}", ButtonStyle.Primary, new Emoji("✏️"));

            return components;
        }
        
        public ComponentBuilder CreateServerActionBehaviorEditorComponents(ServerAction action, ActionBehavior behavior, SocketUser author)
        {
            var components = new ComponentBuilder()
                .WithButton("Set Type", $"SSABT:{action.Id},{author.Id},{behavior.Id}", ButtonStyle.Primary, new Emoji("✏️"))
                .WithButton("Add Condition", $"ASABC:{action.Id},{author.Id},{behavior.Id}", ButtonStyle.Success, new Emoji("➕"))
                .WithButton("Remove Condition", $"RSABC:{action.Id},{author.Id},{behavior.Id}", ButtonStyle.Danger, new Emoji("➖"))
                .WithButton("Edit Condition", $"ESABC:{action.Id},{author.Id},{behavior.Id}", ButtonStyle.Primary, new Emoji("✏️"));

            return components;
        }

        public EmbedBuilder CreateServerActionsListEmbed(SocketGuild guild, ServerAction[] actions, int page = 0)
        {
            var title = $"{guild.Name} Actions";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                .WithColor(Bot.PrimaryColor);
            //25 for list limit!
            for (var i = page * ListLimit; i < actions.Length && i < (page * ListLimit + ListLimit); i++)
            {
                embed.AddField(actions[i].Name, $"`{actions[i].Id}`");
            }

            return embed;
        }

        public EmbedBuilder CreateServerActionBehaviorsEditorEmbed(ServerAction action, SocketUser author, int page = 0)
        {
            var title = $"Editing Action Behaviors {action.Name}";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                .WithColor(Bot.PrimaryColor)
                .WithAuthor(author);

            //25 for list limit!
            for (var i = page * ListLimit; i < action.Behaviors.Count && i < (page * ListLimit + ListLimit); i++)
            {
                embed.AddField(action.Behaviors.ElementAt(i).Type.ToString(), i);
            }

            return embed;
        }

        //Staff Stuff
        public async Task<EmbedBuilder> CreateUserInfoEmbed(SocketUser user, SocketUser author)
        {
            var dbUser = await Database.GetOrCreateUserById(user.Id);
            var embed = new EmbedBuilder()
                .WithTitle(user.GlobalName.Truncate(Bot.TitleLengthLimit))
                .WithColor(Bot.PrimaryColor)
                .WithThumbnailUrl(user.GetDisplayAvatarUrl())
                .AddField("Last Active", dbUser.LastActive.ToDiscordUnixTimestampFormat())
                .WithAuthor(author);

            return embed;
        }
    }
}
