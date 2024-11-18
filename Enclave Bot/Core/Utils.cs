using Discord;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;

namespace Enclave_Bot.Core
{
    public class Utils(DatabaseContext database)
    {
        private readonly DatabaseContext Database = database;

        public Embed CreateErrorEmbed(string errorDescription, IUser author)
        {
            return new EmbedBuilder()
                    .WithTitle("Error!")
                    .WithDescription($"**Error Message:** {errorDescription}")
                    .WithAuthor(author)
                    .WithColor(Constants.ErrorColor).Build();
        }

        //Application Stuff
        public ComponentBuilder CreateApplicationListComponents(Application[] application, SocketUser author, int page = 0)
        {
            var apps = application.Length;

            var components = new ComponentBuilder();

            if (apps > (page + 1) * Constants.ListLimit)
                components.WithButton("Next Page", $"{Constants.APP_LIST_NEXT_PAGE}:{author.Id},{page}", ButtonStyle.Primary, new Emoji("▶️"), row: 1);
            if (page > 0)
                components.WithButton("Previous Page", $"{Constants.APP_LIST_PREVIOUS_PAGE}:{author.Id},{page}", ButtonStyle.Primary, new Emoji("◀️"), row: 1);

            return components;
        }

        public EmbedBuilder CreateApplicationListEmbed(SocketGuild guild, Application[] applications, SocketUser author, int page = 0)
        {
            var title = $"{guild.Name} Applications";
            var embed = new EmbedBuilder()
                .WithAuthor(author)
                .WithTitle(title.Truncate(Constants.TitleLimit))
                .WithColor(Constants.PrimaryColor);

            for (var i = page * Constants.ListLimit; i < applications.Length && i < (page * Constants.ListLimit + Constants.ListLimit); i++)
            {
                embed.AddField(applications[i].Name.Truncate(Constants.TitleLimit), $"`{applications[i].Id}`".Truncate(Constants.ValueLimit));
            }

            return embed;
        }

        public ComponentBuilder CreateApplicationEditorComponents(Application application, SocketUser author, int page = 0)
        {
            var appQuestions = Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).Count();

            var components = new ComponentBuilder()
                .WithButton("Add Question", $"{Constants.ADD_APP_QUESTION}:{author.Id},{application.Id}", ButtonStyle.Success, new Emoji("➕"))
                .WithButton("Remove Question", $"{Constants.REMOVE_APP_QUESTION}:{author.Id},{application.Id},{page}", ButtonStyle.Danger, new Emoji("➖"))
                .WithButton("Edit Question", $"{Constants.EDIT_APP_QUESTION}:{author.Id},{application.Id},{page}", ButtonStyle.Primary, new Emoji("✏️"))
                .WithButton("Edit Actions", $"{Constants.SWITCH_TO_APP_ACTIONS}:{author.Id},{application.Id}", ButtonStyle.Primary, new Emoji("\ud83d\udd04"));

            if (appQuestions > (page + 1) * Constants.ListLimit)
                components.WithButton("Next Page", $"{Constants.APP_QUESTIONS_NEXT_PAGE}:{author.Id},{application.Id},{page}", ButtonStyle.Primary, new Emoji("▶️"), row: 1);
            if (page > 0)
                components.WithButton("Previous Page", $"{Constants.APP_QUESTIONS_PREVIOUS_PAGE}:{author.Id},{application.Id},{page}", ButtonStyle.Primary, new Emoji("◀️"), row: 1);

            return components;
        }

        public EmbedBuilder CreateApplicationEditorEmbed(Application application, SocketUser author, int page = 0)
        {
            var applicationQuestions = Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).ToArray();
            var title = $"Editing Application {application.Name}";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Constants.TitleLimit))
                .WithColor(Constants.PrimaryColor)
                .WithAuthor(author);

            for (int i = page * Constants.ListLimit; i < applicationQuestions.Length && i < (page * Constants.ListLimit + Constants.ListLimit); i++)
            {
                embed.AddField($"{i} - Index: {applicationQuestions[i].Index}, Required: {(applicationQuestions[i].Required ? "Yes" : "No")}".Truncate(Constants.TitleLimit), applicationQuestions[i].Question.Truncate(Constants.ValueLimit));
            }

            return embed;
        }

        public ComponentBuilder CreateApplicationEditorActionComponents(Application application, SocketUser author)
        {
            var components = new ComponentBuilder()
                .WithButton("Add Accept Role", $"{Constants.ADD_APP_ACCEPT_ROLE}:{author.Id}", ButtonStyle.Primary, new Emoji("➕"))
                .WithButton("Remove Accept Role", $"{Constants.REMOVE_APP_ACCEPT_ROLE}:{author.Id}", ButtonStyle.Primary, new Emoji("➖"))
                .WithButton("Set Accept Message", $"{Constants.SET_APP_ACCEPT_MESSAGE}:{author.Id}", ButtonStyle.Primary, new Emoji("✏️"))
                .WithButton("Set Deny Message", $"{Constants.SET_APP_DENY_MESSAGE}:{author.Id}", ButtonStyle.Primary, new Emoji("✏️"))
                .WithButton("Set Submission Channel", $"{Constants.SET_APP_SUBMISSION_CHANNEL}:{author.Id}", ButtonStyle.Primary, new Emoji("✏️"))
                .WithButton("Edit Questions", $"{Constants.SWITCH_TO_APP_QUESTIONS}:{author.Id},{application.Id}", ButtonStyle.Primary, new Emoji("\ud83d\udd04"));

            return components;
        }

        public EmbedBuilder CreateApplicationEditorActionEmbed(Application application, SocketUser author)
        {
            var applicationQuestions = Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).ToArray();
            var title = $"Editing Application {application.Name}";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Constants.TitleLimit))
                .WithColor(Constants.PrimaryColor)
            .WithAuthor(author);

            return embed;
        }

        //Staff Stuff
        public async Task<EmbedBuilder> CreateUserInfoEmbed(SocketUser user, SocketUser author)
        {
            var dbUser = await Database.GetOrCreateUserById(user.Id);
            var embed = new EmbedBuilder()
                .WithTitle(user.GlobalName.Truncate(Constants.TitleLimit))
                .WithColor(Constants.PrimaryColor)
                .WithThumbnailUrl(user.GetDisplayAvatarUrl())
                .AddField("Last Active", dbUser.LastActive.ToDiscordUnixTimestampFormat())
                .WithAuthor(author);

            return embed;
        }
    }
}
