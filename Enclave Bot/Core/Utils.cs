using Discord;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;

namespace Enclave_Bot.Core
{
    public class Utils
    {
        public static Embed CreateErrorEmbed(string errorDescription, IUser author)
        {
            return new EmbedBuilder()
                    .WithTitle("Error!")
                    .WithDescription($"**Error Message:** {errorDescription}")
                    .WithAuthor(author)
                    .WithColor(Color.Red).Build();
        }

        //Application Stuff
        public static ComponentBuilder CreateApplicationEditorComponents(ServerApplication application, SocketUser user)
        {
            var components = new ComponentBuilder()
                .WithButton("Add Question", $"AAQ:{application.Id},{user.Id}", ButtonStyle.Success, new Emoji("➕"))
                .WithButton("Remove Question", $"RAQ:{application.Id},{user.Id}", ButtonStyle.Danger, new Emoji("➖"))
                .WithButton("Edit Question", $"EAQ:{application.Id},{user.Id}", ButtonStyle.Primary, new Emoji("✏️"));

            return components;
        }

        public static EmbedBuilder CreateApplicationListEmbed(SocketGuild guild, ServerApplication[] applications, int page = 0)
        {
            var title = $"{guild.Name} Applications";
            var embed = new EmbedBuilder()
                            .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                            .WithColor(Bot.PrimaryColor);
            //25 for list limit!
            for(int i = page * Bot.ListLimit; i < applications.Length; i++)
            {
                embed.AddField(applications[i].Name, $"`{applications[i].Id}`");
            }

            return embed;
        }

        public static EmbedBuilder CreateApplicationEditorEmbed(ServerApplication application, SocketUser user)
        {
            var title = $"Editing Application {application.Name}";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                .WithColor(Bot.PrimaryColor)
            .WithAuthor(user);

            for (int i = 0; i < application.Questions.Count && i < Bot.QuestionsLimit; i++)
            {
                embed.AddField(application.Questions[i], i);
            }

            return embed;
        }

        //Actions Stuff
        public static ComponentBuilder CreateServerBehaviorsEditorComponents(ServerAction action, SocketUser user)
        {
            var components = new ComponentBuilder()
                .WithButton("Add Action", $"ASAB:{action.Id},{user.Id}", ButtonStyle.Success, new Emoji("➕"))
                .WithButton("Remove Action", $"RSAB:{action.Id},{user.Id}", ButtonStyle.Danger, new Emoji("➖"))
                .WithButton("Edit Action", $"ESAB:{action.Id},{user.Id}", ButtonStyle.Primary, new Emoji("✏️"))
                .WithButton("Switch To Conditions", $"SSAC:{action.Id},{user.Id}", ButtonStyle.Primary, new Emoji("🔄️"));

            return components;
        }

        public static EmbedBuilder CreateServerActionsListEmbed(SocketGuild guild, ServerAction[] actions, int page = 0)
        {
            var title = $"{guild.Name} Actions";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                .WithColor(Bot.PrimaryColor);
            //25 for list limit!
            for (int i = page * Bot.ListLimit; i < actions.Length; i++)
            {
                embed.AddField(actions[i].Name, $"`{actions[i].Id}`");
            }

            return embed;
        }

        public static EmbedBuilder CreateServerBehaviorsEditorEmbed(ServerAction action, SocketUser user)
        {
            var title = $"Editing Action {action.Name}";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                .WithColor(Bot.PrimaryColor)
                .WithAuthor(user);

            //25 for list limit!
            for (int i = 0; i < action.Behaviors.Count && i < Bot.BehaviorsLimit; i++)
            {
                embed.AddField(action.Behaviors.ElementAt(i).Type.ToString(), i);
            }

            return embed;
        }
    }
}
