using System.Text;
using Discord;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;

namespace Enclave_Bot.Core
{
    public static class Utils
    {
        public static Embed CreateError(string errorDescription, IUser author)
        {
            return new EmbedBuilder()
                .WithTitle("Error!")
                .WithDescription($"**Error Message:** {errorDescription}")
                .WithAuthor(author)
                .WithColor(Constants.ErrorColor).Build();
        }

        public static Embed CreateUserInfo(IUser user, User botUser, IUser author)
        {
            var embed = new EmbedBuilder()
                .WithTitle(user.GlobalName.Truncate(Constants.EMBED_TITLE_CHARACTER_LIMIT))
                .WithColor(Constants.PrimaryColor)
                .WithThumbnailUrl(user.GetDisplayAvatarUrl())
                .AddField("Last Active", botUser.LastActive.ToDiscordUnixTimestampFormat())
                .AddField("Created At", $"{user.CreatedAt} - <t:{user.CreatedAt.ToUnixTimeSeconds()}:R>")
                .WithAuthor(author);

            if (user is SocketGuildUser guildUser)
            {
                embed.AddField("Joined At", $"{guildUser.JoinedAt} - <t:{guildUser.JoinedAt?.ToUnixTimeSeconds()}:R>");
            }

            return embed.Build();
        }
        
        #region Application Utils
        public static (Embed, MessageComponent) CreateApplicationList(Application[] applications, int page, IUser author)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Applications")
                .WithColor(Constants.PrimaryColor)
                .WithAuthor(author);
            var editSelectMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.APPLICATION_LIST_EDIT}:{author.Id}")
                .WithPlaceholder("Edit application");
            var deleteSelectMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.APPLICATION_LIST_DELETE}:{author.Id}")
                .WithPlaceholder("Delete application");
            var components = new ComponentBuilder();

            var limit = Math.Min(Constants.EMBED_FIELDS_LIMIT, Constants.SELECT_MENU_OPTIONS_LIMIT);
            var i = page * Constants.EMBED_FIELDS_LIMIT;
            while (i < applications.Length && i < (page + 1) * limit)
            {
                var application = applications.ElementAt(i);
                embed.AddField(application.Name.Truncate(Constants.EMBED_FIELD_NAME_CHARACTER_LIMIT),
                    $"`{application.Id}`".Truncate(Constants.EMBED_FIELD_VALUE_CHARACTER_LIMIT));
                editSelectMenu.AddOption(application.Name.Truncate(Constants.SELECT_MENU_OPTION_LABEL_CHARACTER_LIMIT), application.Id.ToString(),
                    application.Id.ToString());
                deleteSelectMenu.AddOption(application.Name.Truncate(Constants.SELECT_MENU_OPTION_LABEL_CHARACTER_LIMIT), application.Id.ToString(),
                    application.Id.ToString());
                i++;
            }

            if (editSelectMenu.Options.Count > 0)
                components.WithSelectMenu(editSelectMenu);
            if (deleteSelectMenu.Options.Count > 0)
                components.WithSelectMenu(deleteSelectMenu);


            components.WithButton("Create", $"{Constants.APPLICATION_LIST_CREATE}:{author.Id}", ButtonStyle.Success, new Emoji("\u2795"));
            if (i > limit)
                components.WithButton("Previous page", $"{Constants.APPLICATION_LIST_NAVIGATE}:{author.Id},{page - 1}", ButtonStyle.Primary, new Emoji("◀️"));
            if (i < applications.Length)
                components.WithButton("Next page", $"{Constants.APPLICATION_LIST_NAVIGATE}:{author.Id},{page + 1}", ButtonStyle.Primary, new Emoji("▶️"));

            return (embed.Build(), components.Build());
        }

        public static (Embed, MessageComponent) CreateApplicationQuestionEditor(Application application, int page, IUser author)
        {
            var orderedQuestions = application.Questions.OrderBy(x => x.Index).ToList();
            var embed = new EmbedBuilder()
                .WithTitle($"Editing Application {application.Name}".Truncate(Constants.EMBED_TITLE_CHARACTER_LIMIT))
                .WithColor(Constants.PrimaryColor)
                .WithAuthor(author);
            var editSelectMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.APPLICATION_EDIT_EDIT_QUESTION}:{author.Id},{application.Id}")
                .WithPlaceholder("Edit question");
            var deleteSelectMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.APPLICATION_EDIT_DELETE_QUESTION}:{author.Id},{application.Id}")
                .WithPlaceholder("Delete question");
            var components = new ComponentBuilder();

            var limit = Math.Min(Constants.EMBED_FIELDS_LIMIT, Constants.SELECT_MENU_OPTIONS_LIMIT);
            var i = page * limit;
            while (i < orderedQuestions.Count && i < (page + 1) * limit)
            {
                var question = orderedQuestions.ElementAt(i);
                embed.AddField(question.Question.Truncate(Constants.EMBED_FIELD_NAME_CHARACTER_LIMIT),
                    $"Id: {question.Id}\nIndex: {question.Index}\nRequired: {question.Required}".Truncate(Constants.EMBED_FIELD_VALUE_CHARACTER_LIMIT));
                editSelectMenu.AddOption(question.Question.Truncate(Constants.SELECT_MENU_OPTION_LABEL_CHARACTER_LIMIT), question.Id.ToString(),
                    question.Id.ToString());
                deleteSelectMenu.AddOption(question.Question.Truncate(Constants.SELECT_MENU_OPTION_LABEL_CHARACTER_LIMIT), question.Id.ToString(),
                    question.Id.ToString());
                i++;
            }

            if (editSelectMenu.Options.Count > 0)
                components.WithSelectMenu(editSelectMenu);
            if (deleteSelectMenu.Options.Count > 0)
                components.WithSelectMenu(deleteSelectMenu);

            components.WithButton("Create", $"{Constants.APPLICATION_EDIT_CREATE_QUESTION}:{author.Id},{application.Id}", ButtonStyle.Success,
                new Emoji("\u2795"));
            components.WithButton("Edit Actions", $"{Constants.APPLICATION_EDIT_ACTIONS}:{author.Id},{application.Id}", ButtonStyle.Primary,
                new Emoji("\u270f\ufe0f"));
            if (i > limit)
                components.WithButton("Previous page", $"{Constants.APPLICATION_EDIT_NAVIGATE_QUESTIONS}:{author.Id},{application.Id},{page - 1}",
                    ButtonStyle.Primary, new Emoji("◀️"));
            if (i < orderedQuestions.Count)
                components.WithButton("Next page", $"{Constants.APPLICATION_EDIT_NAVIGATE_QUESTIONS}:{author.Id},{application.Id},{page + 1}",
                    ButtonStyle.Primary, new Emoji("▶️"));

            return (embed.Build(), components.Build());
        }

        public static (Embed, MessageComponent) CreateApplicationActionEditor(Application application, IUser author, IGuild guild)
        {
            var embed = new EmbedBuilder()
                .WithTitle($"Editing Application {application.Name}".Truncate(Constants.EMBED_TITLE_CHARACTER_LIMIT))
                .WithColor(Constants.PrimaryColor)
                .WithAuthor(author);
            var submissionChannelSelectMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.APPLICATION_EDIT_SET_SUBMISSION_CHANNEL}:{author.Id},{application.Id}")
                .WithType(ComponentType.ChannelSelect)
                .WithMinValues(0)
                .WithChannelTypes(ChannelType.Text)
                .WithPlaceholder("Set submission channel");
            var addRolesSelectMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.APPLICATION_EDIT_SET_ADD_ROLES}:{author.Id},{application.Id}")
                .WithType(ComponentType.RoleSelect)
                .WithMinValues(0)
                .WithMaxValues(Constants.SELECT_MENU_OPTIONS_LIMIT)
                .WithPlaceholder("Set add roles");
            var removeRolesSelectMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.APPLICATION_EDIT_SET_REMOVE_ROLES}:{author.Id},{application.Id}")
                .WithType(ComponentType.RoleSelect)
                .WithMinValues(0)
                .WithMaxValues(Constants.SELECT_MENU_OPTIONS_LIMIT)
                .WithPlaceholder("Set remove roles");
            var failActionSelectMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.APPLICATION_EDIT_SET_FAIL_MODE}:{author.Id},{application.Id}")
                .WithPlaceholder("Set fail action");
            var components = new ComponentBuilder();
            
            if (application.SubmissionChannel != null)
                submissionChannelSelectMenu.AddDefaultValue((ulong)application.SubmissionChannel, SelectDefaultValueType.Channel);

            var addRolesString = new StringBuilder();
            var removeRolesString = new StringBuilder();
            if (application.AddRoles.Count <= 0)
                addRolesString.AppendLine("No roles set.");
            else
            {
                foreach (var roleId in application.AddRoles)
                {
                    if (guild.GetRole(roleId) == null) continue;
                    addRolesSelectMenu.AddDefaultValue(roleId, SelectDefaultValueType.Role);
                    addRolesString.AppendLine($"<@&{roleId}>");
                }
            }

            if (application.RemoveRoles.Count <= 0)
            {
                removeRolesString.AppendLine("No roles set.");
            }
            else
            {
                foreach (var roleId in application.RemoveRoles)
                {
                    if (guild.GetRole(roleId) == null) continue;
                    removeRolesSelectMenu.AddDefaultValue(roleId, SelectDefaultValueType.Role);
                    addRolesString.AppendLine($"<@&{roleId}>");
                }
            }

            foreach (var failAction in Enum.GetValues<ApplicationFailAction>())
            {
                failActionSelectMenu.AddOption(Enum.GetName(failAction).Truncate(Constants.SELECT_MENU_OPTION_LABEL_CHARACTER_LIMIT),
                    ((int)failAction).ToString(), isDefault: failAction == application.FailAction);
            }

            embed.AddField("Submission Channel", application.SubmissionChannel == null? "No Channel Set." : $"<#{application.SubmissionChannel}>");
            embed.AddField("Add Roles", addRolesString.ToString().Truncate(Constants.EMBED_FIELD_VALUE_CHARACTER_LIMIT));
            embed.AddField("Remove Roles", removeRolesString.ToString().Truncate(Constants.EMBED_FIELD_VALUE_CHARACTER_LIMIT));
            embed.AddField("Retries",
                application.Retries <= 0 ? "Infinite" : application.Retries.ToString().Truncate(Constants.EMBED_FIELD_VALUE_CHARACTER_LIMIT));
            embed.AddField("Fail Action", application.FailAction.ToString().Truncate(Constants.EMBED_FIELD_VALUE_CHARACTER_LIMIT));
            components.WithSelectMenu(submissionChannelSelectMenu);
            components.WithSelectMenu(addRolesSelectMenu);
            components.WithSelectMenu(removeRolesSelectMenu);
            components.WithSelectMenu(failActionSelectMenu);
            components.WithButton("Set Retries", $"{Constants.APPLICATION_EDIT_SET_RETRIES}:{author.Id},{application.Id}", ButtonStyle.Primary,
                new Emoji("\ud83d\udd04"));
            components.WithButton("Edit Questions", $"{Constants.APPLICATION_EDIT_QUESTIONS}:{author.Id},{application.Id}", ButtonStyle.Primary,
                new Emoji("\u270f\ufe0f"));

            return (embed.Build(), components.Build());
        }
        #endregion
        
        #region Server Settings Utils

        public static (Embed, MessageComponent) CreateServerSettingsViewer(ServerSettings serverSettings, IUser author, IGuild guild)
        {
            var embed = new EmbedBuilder()
                .WithTitle($"{guild.Name} Server Settings".Truncate(Constants.EMBED_TITLE_CHARACTER_LIMIT))
                .WithColor(Constants.PrimaryColor)
                .WithAuthor(author);
            var staffRolesSelectMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.SERVER_SETTINGS_SET_STAFF_ROLES}:{author.Id}")
                .WithType(ComponentType.RoleSelect)
                .WithMinValues(0)
                .WithMaxValues(Constants.SELECT_MENU_OPTIONS_LIMIT)
                .WithPlaceholder("Set staff roles");
            var components = new ComponentBuilder();
            
            var staffRolesString = new StringBuilder();
            if (serverSettings.StaffRoles.Count <= 0)
                staffRolesString.AppendLine("No roles set.");
            else
            {
                foreach (var roleId in serverSettings.StaffRoles)
                {
                    if (guild.GetRole(roleId) == null) continue;
                    staffRolesSelectMenu.AddDefaultValue(roleId, SelectDefaultValueType.Role);
                    staffRolesString.AppendLine($"<@&{roleId}>");
                }
            }
            
            embed.AddField("Staff Roles", staffRolesString.ToString().Truncate(Constants.EMBED_FIELD_VALUE_CHARACTER_LIMIT));
            components.WithSelectMenu(staffRolesSelectMenu);
            return (embed.Build(), components.Build());
        }
        #endregion
        
        #region Server Actions Utils

        public static (Embed, MessageComponent) CreateServerActionsList(ServerActionGroup[] actions, int page, IUser author)
        {
            var embed = new EmbedBuilder()
                .WithTitle("Actions")
                .WithColor(Constants.PrimaryColor)
                .WithAuthor(author);
            var editSelectMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.SERVER_ACTION_LIST_EDIT}:{author.Id}")
                .WithPlaceholder("Edit action");
            var deleteSelectMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.SERVER_ACTION_LIST_DELETE}:{author.Id}")
                .WithPlaceholder("Delete action");
            var components = new ComponentBuilder();
            
            var limit = Math.Min(Constants.EMBED_FIELDS_LIMIT, Constants.SELECT_MENU_OPTIONS_LIMIT);
            var i = page * Constants.EMBED_FIELDS_LIMIT;
            while (i < actions.Length && i < (page + 1) * limit)
            {
                var application = actions.ElementAt(i);
                embed.AddField(application.Name.Truncate(Constants.EMBED_FIELD_NAME_CHARACTER_LIMIT),
                    $"`{application.Id}`".Truncate(Constants.EMBED_FIELD_VALUE_CHARACTER_LIMIT));
                editSelectMenu.AddOption(application.Name.Truncate(Constants.SELECT_MENU_OPTION_LABEL_CHARACTER_LIMIT), application.Id.ToString(),
                    application.Id.ToString());
                deleteSelectMenu.AddOption(application.Name.Truncate(Constants.SELECT_MENU_OPTION_LABEL_CHARACTER_LIMIT), application.Id.ToString(),
                    application.Id.ToString());
                i++;
            }

            if (editSelectMenu.Options.Count > 0)
                components.WithSelectMenu(editSelectMenu);
            if (deleteSelectMenu.Options.Count > 0)
                components.WithSelectMenu(deleteSelectMenu);


            components.WithButton("Create", $"{Constants.SERVER_ACTION_LIST_CREATE}:{author.Id}", ButtonStyle.Success, new Emoji("\u2795"));
            if (i > limit)
                components.WithButton("Previous page", $"{Constants.SERVER_ACTION_LIST_NAVIGATE}:{author.Id},{page - 1}", ButtonStyle.Primary, new Emoji("◀️"));
            if (i < actions.Length)
                components.WithButton("Next page", $"{Constants.SERVER_ACTION_LIST_NAVIGATE}:{author.Id},{page + 1}", ButtonStyle.Primary, new Emoji("▶️"));

            return (embed.Build(), components.Build());
        }
        #endregion
    }
}