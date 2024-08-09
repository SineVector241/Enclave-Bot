using Discord.Interactions;
using Discord;
using Enclave_Bot.Database;
using Discord.WebSocket;
using Enclave_Bot.Extensions;
using Fergun.Interactive;

namespace Enclave_Bot.Core.Application
{
    public class Buttons(DatabaseContext database, InteractiveService interactive) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        private readonly InteractiveService Interactive = interactive;
        private readonly DatabaseContext Database = database;

        [ComponentInteraction("AAQ:*,*")]
        public async Task AddAppQuestion(string applicationId, string userId)
        {
            var appId = Guid.Parse(applicationId);
            var ownerId = ulong.Parse(userId);

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);
            var application = Database.ServerApplications.Where(x => x.ServerId == server.Id).FirstOrDefault(x => x.Id == appId);

            if (ownerId != Context.User.Id)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed("You are not the owner of this application editor!", Context.User), ephemeral: true);
                return;
            }
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The application `{applicationId}` no longer exists!", Context.User), ephemeral: true);
                return;
            }

            var title = $"Adding Question: {application.Name}";
            await Context.Interaction.RespondWithModalAsync<AddAppQuestionModal>($"AAQM:{application.Id}", RequestOptions.Default, x => x.WithTitle(title.Truncate(Bot.TitleLengthLimit)));
        }

        [ComponentInteraction("RAQ:*,*")]
        public async Task RemoveAppQuestion(string applicationId, string userId)
        {
            var appId = Guid.Parse(applicationId);
            var ownerId = ulong.Parse(userId);

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);
            var application = Database.ServerApplications.Where(x => x.ServerId == server.Id).FirstOrDefault(x => x.Id == appId);

            if (ownerId != Context.User.Id)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed("You are not the owner of this application editor!", Context.User), ephemeral: true);
                return;
            }
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The application `{applicationId}` no longer exists!", Context.User), ephemeral: true);
                return;
            }

            var questionsSelectMenu = new SelectMenuBuilder()
                .WithCustomId($"DAQ:{application.Id},{Context.Interaction.Message.Id}")
                .WithMinValues(1)
                .WithMaxValues(application.Questions.Count);

            for (var i = 0; i < application.Questions.Count; i++)
            {
                questionsSelectMenu.AddOption(i.ToString(), i.ToString(), application.Questions[i]);
            }

            var components = new ComponentBuilder()
                .WithSelectMenu(questionsSelectMenu);

            await Context.Interaction.RespondOrFollowupAsync("Select the questions you would like to remove.", ephemeral: true, components: components.Build());
        }

        [ComponentInteraction("EAQ:*,*")]
        public async Task EditAppQuestion(string applicationId, string userId)
        {
            var appId = Guid.Parse(applicationId);
            var ownerId = ulong.Parse(userId);

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);
            var application = Database.ServerApplications.Where(x => x.ServerId == server.Id).FirstOrDefault(x => x.Id == appId);

            if (ownerId != Context.User.Id)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed("You are not the owner of this application editor!", Context.User), ephemeral: true);
                return;
            }
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The application `{applicationId}` no longer exists!", Context.User), ephemeral: true);
                return;
            }

            var questionsSelectMenu = new SelectMenuBuilder()
                .WithCustomId($"ESAQ:{application.Id},{Context.Interaction.Message.Id}");

            for (var i = 0; i < application.Questions.Count; i++)
            {
                questionsSelectMenu.AddOption(i.ToString(), i.ToString(), application.Questions[i]);
            }

            var components = new ComponentBuilder()
                .WithSelectMenu(questionsSelectMenu);

            await Context.Interaction.RespondOrFollowupAsync("Select the question you would like to edit.", ephemeral: true, components: components.Build());
        }
    }
}
