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
            var appQuestions = Database.ServerApplicationQuestions.Count(x => x.ApplicationId == application.Id);

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
                .WithButton("Add Addition Role", $"{Constants.ADD_APP_ADDITION_ROLE}:{author.Id},{application.Id}", ButtonStyle.Primary, new Emoji("➕"))
                .WithButton("Remove Addition Role", $"{Constants.REMOVE_APP_ADDITION_ROLE}:{author.Id},{application.Id}", ButtonStyle.Primary, new Emoji("➖"))
                .WithButton("Add Removal Role", $"{Constants.ADD_APP_REMOVAL_ROLE}:{author.Id},{application.Id}", ButtonStyle.Primary, new Emoji("➕"))
                .WithButton("Remove Removal Role", $"{Constants.REMOVE_APP_REMOVAL_ROLE}:{author.Id},{application.Id}", ButtonStyle.Primary, new Emoji("➖"))
                .WithButton("Set Accept Message", $"{Constants.SET_APP_ACCEPT_MESSAGE}:{author.Id},{application.Id}", ButtonStyle.Primary, new Emoji("✏️"), row: 1)
                .WithButton("Set Submission Channel", $"{Constants.SET_APP_SUBMISSION_CHANNEL}:{author.Id},{application.Id}", ButtonStyle.Primary, new Emoji("✏️"), row: 1)
                .WithButton("Set Retries", $"{Constants.SET_APP_RETRIES}:{author.Id},{application.Id}", ButtonStyle.Primary, new Emoji("🧮"), row: 1)
                .WithButton("Set Fail Action", $"{Constants.SET_APP_FAIL_ACTION}:{author.Id},{application.Id}", ButtonStyle.Primary, new Emoji("🔨"), row: 1)
                .WithButton("Edit Questions", $"{Constants.SWITCH_TO_APP_QUESTIONS}:{author.Id},{application.Id}", ButtonStyle.Primary, new Emoji("\ud83d\udd04"), row: 1);

            return components;
        }

        public EmbedBuilder CreateApplicationEditorActionEmbed(Application application, SocketUser author)
        {
            var title = $"Editing Application {application.Name}";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Constants.TitleLimit))
                .AddField("Accept Message", string.IsNullOrWhiteSpace(application.AcceptMessage) ? "N.A." : application.AcceptMessage.Truncate(Constants.ValueLimit))
                .AddField("Submission Channel", application.SubmissionChannel == null? "N.A." : $"<#{application.SubmissionChannel}>")
                .AddField("Retries", application.Retries.ToString())
                .AddField("Fail Action", application.FailAction.ToString())
                .WithColor(Constants.PrimaryColor)
                .WithAuthor(author);

            var addRoles = application.AddRoles.Aggregate(string.Empty, (current, addRole) => current + $"<@&{addRole}>\n");
            embed.AddField("Adds Roles", string.IsNullOrWhiteSpace(addRoles) ? "None" : addRoles);

            var removeRoles = application.RemoveRoles.Aggregate(string.Empty, (current, removeRole) => current + $"<@&{removeRole}>\n");
            embed.AddField("Removes Roles", string.IsNullOrWhiteSpace(removeRoles) ? "None" : removeRoles);
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
                .AddField("Created At", $"{user.CreatedAt} - <t:{user.CreatedAt.ToUnixTimeSeconds()}:R>")
                .WithAuthor(author);

            if(user is SocketGuildUser guildUser)
            {
                embed.AddField("Joined At", $"{guildUser.JoinedAt} - <t:{guildUser.JoinedAt?.ToUnixTimeSeconds()}:R>");
            }

            return embed;
        }
    }
}
