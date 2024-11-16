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
                    .WithColor(Color.Red).Build();
        }

        //Application Stuff
        public ComponentBuilder CreateApplicationEditorComponents(Application application, SocketUser author)
        {
            var components = new ComponentBuilder()
                .WithButton("Add Question", $"AAQ:{application.Id},{author.Id}", ButtonStyle.Success, new Emoji("➕"))
                .WithButton("Remove Question", $"RAQ:{application.Id},{author.Id}", ButtonStyle.Danger, new Emoji("➖"))
                .WithButton("Edit Question", $"EAQ:{application.Id},{author.Id}", ButtonStyle.Primary, new Emoji("✏️"));

            return components;
        }

        public EmbedBuilder CreateApplicationListEmbed(SocketGuild guild, Application[] applications, int page = 0)
        {
            var title = $"{guild.Name} Applications";
            var embed = new EmbedBuilder()
                            .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                            .WithColor(Bot.PrimaryColor);
            //25 for list limit!
            for(var i = page * Bot.ListLimit; i < applications.Length; i++)
            {
                embed.AddField(applications[i].Name, $"`{applications[i].Id}`");
            }

            return embed;
        }

        public EmbedBuilder CreateApplicationEditorEmbed(Application application, SocketUser author)
        {
            var title = $"Editing Application {application.Name}";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                .WithColor(Bot.PrimaryColor)
            .WithAuthor(author);

            for (int i = 0; i < application.Questions.Count && i < Bot.QuestionsLimit; i++)
            {
                //embed.AddField([i], i);
            }

            return embed;
        }

        //Actions Stuff
        public ComponentBuilder CreateServerActionBehaviorsEditorComponents(ServerAction action, SocketUser author)
        {
            var components = new ComponentBuilder()
                .WithButton("Add Behavior", $"ASAB:{action.Id},{author.Id}", ButtonStyle.Success, new Emoji("➕"))
                .WithButton("Remove Behavior", $"RSAB:{action.Id},{author.Id}", ButtonStyle.Danger, new Emoji("➖"))
                .WithButton("Edit Behavior", $"ESAB:{action.Id},{author.Id}", ButtonStyle.Primary, new Emoji("✏️"))
                .WithButton("Switch To Conditions", $"SSAC:{action.Id},{author.Id}", ButtonStyle.Primary, new Emoji("\ud83d\udd04"));

            return components;
        }
        
        public ComponentBuilder CreateServerActionConditionsEditorComponents(ServerAction action, SocketUser author)
        {
            var components = new ComponentBuilder()
                .WithButton("Add Condition", $"ASAC:{action.Id},{author.Id}", ButtonStyle.Success, new Emoji("➕"))
                .WithButton("Remove Condition", $"RSAC:{action.Id},{author.Id}", ButtonStyle.Danger, new Emoji("➖"))
                .WithButton("Edit Condition", $"ESAC:{action.Id},{author.Id}", ButtonStyle.Primary, new Emoji("✏️"))
                .WithButton("Switch To Actions", $"SSAB:{action.Id},{author.Id}", ButtonStyle.Primary, new Emoji("\ud83d\udd04"));

            return components;
        }

        public EmbedBuilder CreateServerActionsListEmbed(SocketGuild guild, ServerAction[] actions, int page = 0)
        {
            var title = $"{guild.Name} Actions";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                .WithColor(Bot.PrimaryColor);
            //25 for list limit!
            for (var i = page * Bot.ListLimit; i < actions.Length; i++)
            {
                embed.AddField(actions[i].Name, $"`{actions[i].Id}`");
            }

            return embed;
        }

        public EmbedBuilder CreateServerActionBehaviorsEditorEmbed(ServerAction action, SocketUser author)
        {
            var title = $"Editing Action Behaviors {action.Name}";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                .WithColor(Bot.PrimaryColor)
                .WithAuthor(author);

            //25 for list limit!
            for (var i = 0; i < action.Behaviors.Count && i < Bot.BehaviorsLimit; i++)
            {
                embed.AddField(action.Behaviors.ElementAt(i).Type.ToString(), i);
            }

            return embed;
        }
        
        public EmbedBuilder CreateServerActionConditionsEditorEmbed(ServerAction action, SocketUser author)
        {
            var title = $"Editing Action Conditions {action.Name}";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                .WithColor(Bot.PrimaryColor)
                .WithAuthor(author);

            //25 for list limit!
            for (var i = 0; i < action.Behaviors.Count && i < Bot.BehaviorsLimit; i++)
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
