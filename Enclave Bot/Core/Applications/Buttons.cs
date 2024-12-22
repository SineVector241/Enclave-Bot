using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Preconditions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Applications
{
    [IsStaff]
    public class Buttons(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction($"{Constants.APPLICATION_LIST_NAVIGATE}:*,*")]
        public async Task ApplicationListNavigate(string sAuthorId, string sPage)
        {
            var authorId = ulong.Parse(sAuthorId);
            var page = int.Parse(sPage);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this application list!", ephemeral: true);
                return;
            }

            if (page < 0)
            {
                await RespondAsync("Page must be greater than or equal to zero", ephemeral: true);
                return;
            }

            var applications = (await database.Servers
                .Include(x => x.ApplicationSettings)
                .ThenInclude(x => x.Applications)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ApplicationSettings.Applications;
            await DeferAsync();

            //Don't really care if it fails.
            var applicationList = Utils.CreateApplicationList(applications.ToArray(), page, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationList.Item1;
                x.Components = applicationList.Item2;
            });
        }

        [ComponentInteraction($"{Constants.APPLICATION_LIST_CREATE}:*")]
        public async Task ApplicationListCreate(string sAuthorId)
        {
            var authorId = ulong.Parse(sAuthorId);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this application list!", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<CreateApplicationModal>($"{Constants.APPLICATION_MODAL_CREATE}");
        }

        [ComponentInteraction($"{Constants.APPLICATION_EDIT_CREATE_QUESTION}:*,*")]
        public async Task ApplicationCreateQuestion(string sAuthorId, string sApplicationId)
        {
            var authorId = ulong.Parse(sAuthorId);
            var applicationId = Guid.Parse(sApplicationId);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this application list!", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<ApplicationQuestionModal>($"{Constants.APPLICATION_MODAL_CREATE_QUESTION}:{applicationId}");
        }

        [ComponentInteraction($"{Constants.APPLICATION_EDIT_NAVIGATE_QUESTIONS}:*,*,*")]
        public async Task ApplicationNavigateQuestions(string sAuthorId, string sApplicationId, string sPage)
        {
            var authorId = ulong.Parse(sAuthorId);
            var applicationId = Guid.Parse(sApplicationId);
            var page = int.Parse(sPage);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this application list!", ephemeral: true);
                return;
            }

            if (page < 0)
            {
                await RespondAsync("Page must be greater than or equal to zero", ephemeral: true);
                return;
            }

            var applications = (await database.Servers
                .Include(x => x.ApplicationSettings)
                .ThenInclude(x => x.Applications)
                .ThenInclude(application => application.Questions)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ApplicationSettings.Applications;
            var application = applications.FirstOrDefault(x => x.Id == applicationId);

            if (application == null)
            {
                await RespondAsync($"Application with id {applicationId} was not found!", ephemeral: true);
                return;
            }

            await DeferAsync();

            //Don't really care if it fails.
            var applicationEditor = Utils.CreateApplicationQuestionEditor(application, page, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationEditor.Item1;
                x.Components = applicationEditor.Item2;
            });
        }

        [ComponentInteraction($"{Constants.APPLICATION_EDIT_ACTIONS}:*,*")]
        public async Task ApplicationEditActions(string sAuthorId, string sApplicationId)
        {
            var authorId = ulong.Parse(sAuthorId);
            var applicationId = Guid.Parse(sApplicationId);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this application list!", ephemeral: true);
                return;
            }

            var applications = (await database.Servers
                .Include(x => x.ApplicationSettings)
                .ThenInclude(x => x.Applications)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ApplicationSettings.Applications;
            var application = applications.FirstOrDefault(x => x.Id == applicationId);

            if (application == null)
            {
                await RespondAsync($"Application with id {applicationId} was not found!", ephemeral: true);
                return;
            }

            await DeferAsync();

            //Don't really care if it fails.
            var applicationEditor = Utils.CreateApplicationActionEditor(application, Context.User, Context.Guild);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationEditor.Item1;
                x.Components = applicationEditor.Item2;
            });
        }
        
        [ComponentInteraction($"{Constants.APPLICATION_EDIT_SET_RETRIES}:*,*")]
        public async Task ApplicationEditSetRetries(string sAuthorId, string sApplicationId)
        {
            var authorId = ulong.Parse(sAuthorId);
            var applicationId = Guid.Parse(sApplicationId);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this application list!", ephemeral: true);
                return;
            }

            var applications = (await database.Servers
                .Include(x => x.ApplicationSettings)
                .ThenInclude(x => x.Applications)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ApplicationSettings.Applications;
            var application = applications.FirstOrDefault(x => x.Id == applicationId);

            if (application == null)
            {
                await RespondAsync($"Application with id {applicationId} was not found!", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<ApplicationRetriesModal>($"{Constants.APPLICATION_MODAL_SET_RETRIES}:{applicationId}",
                modifyModal: modal => modal.UpdateTextInput("retries", application.Retries));
        }

        [ComponentInteraction($"{Constants.APPLICATION_EDIT_QUESTIONS}:*,*")]
        public async Task ApplicationEditQuestions(string sAuthorId, string sApplicationId)
        {
            var authorId = ulong.Parse(sAuthorId);
            var applicationId = Guid.Parse(sApplicationId);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this application list!", ephemeral: true);
                return;
            }

            var applications = (await database.Servers
                .Include(x => x.ApplicationSettings)
                .ThenInclude(x => x.Applications)
                .ThenInclude(application => application.Questions)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ApplicationSettings.Applications;
            var application = applications.FirstOrDefault(x => x.Id == applicationId);

            if (application == null)
            {
                await RespondAsync($"Application with id {applicationId} was not found!", ephemeral: true);
                return;
            }

            await DeferAsync();

            //Don't really care if it fails.
            var applicationEditor = Utils.CreateApplicationQuestionEditor(application, 0, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationEditor.Item1;
                x.Components = applicationEditor.Item2;
            });
        }
    }
}