using Discord;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;

namespace Enclave_Bot.Core
{
    public static class Utils
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
            for(var i = page * Bot.ListLimit; i < applications.Length; i++)
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
        public static ComponentBuilder CreateServerActionBehaviorsEditorComponents(ServerAction action, SocketUser user)
        {
            var components = new ComponentBuilder()
                .WithButton("Add Behavior", $"ASAB:{action.Id},{user.Id}", ButtonStyle.Success, new Emoji("➕"))
                .WithButton("Remove Behavior", $"RSAB:{action.Id},{user.Id}", ButtonStyle.Danger, new Emoji("➖"))
                .WithButton("Edit Behavior", $"ESAB:{action.Id},{user.Id}", ButtonStyle.Primary, new Emoji("✏️"))
                .WithButton("Switch To Conditions", $"SSAC:{action.Id},{user.Id}", ButtonStyle.Primary, new Emoji("\ud83d\udd04"));

            return components;
        }
        
        public static ComponentBuilder CreateServerActionConditionsEditorComponents(ServerAction action, SocketUser user)
        {
            var components = new ComponentBuilder()
                .WithButton("Add Condition", $"ASAC:{action.Id},{user.Id}", ButtonStyle.Success, new Emoji("➕"))
                .WithButton("Remove Condition", $"RSAC:{action.Id},{user.Id}", ButtonStyle.Danger, new Emoji("➖"))
                .WithButton("Edit Condition", $"ESAC:{action.Id},{user.Id}", ButtonStyle.Primary, new Emoji("✏️"))
                .WithButton("Switch To Actions", $"SSAB:{action.Id},{user.Id}", ButtonStyle.Primary, new Emoji("\ud83d\udd04"));

            return components;
        }

        public static EmbedBuilder CreateServerActionsListEmbed(SocketGuild guild, ServerAction[] actions, int page = 0)
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

        public static EmbedBuilder CreateServerActionBehaviorsEditorEmbed(ServerAction action, SocketUser user)
        {
            var title = $"Editing Action Behaviors {action.Name}";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                .WithColor(Bot.PrimaryColor)
                .WithAuthor(user);

            //25 for list limit!
            for (var i = 0; i < action.Behaviors.Count && i < Bot.BehaviorsLimit; i++)
            {
                embed.AddField(action.Behaviors.ElementAt(i).Type.ToString(), i);
            }

            return embed;
        }
        
        public static EmbedBuilder CreateServerActionConditionsEditorEmbed(ServerAction action, SocketUser user)
        {
            var title = $"Editing Action Conditions {action.Name}";
            var embed = new EmbedBuilder()
                .WithTitle(title.Truncate(Bot.TitleLengthLimit))
                .WithColor(Bot.PrimaryColor)
                .WithAuthor(user);

            //25 for list limit!
            for (var i = 0; i < action.Behaviors.Count && i < Bot.BehaviorsLimit; i++)
            {
                embed.AddField(action.Behaviors.ElementAt(i).Type.ToString(), i);
            }

            return embed;
        }
    }
}
