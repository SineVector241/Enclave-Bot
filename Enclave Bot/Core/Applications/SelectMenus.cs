using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Applications
{
    public class SelectMenus(DatabaseContext database, Utils utils) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        private readonly DatabaseContext Database = database;
        private readonly Utils Utils = utils;

        [ComponentInteraction($"{Constants.REMOVE_APP_QUESTION_SELECTION}:*,*,*")]
        public async Task RemoveQuestion(string author, string applicationId, string page, string[] value)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var pageN = int.Parse(page);
            var selectedQuestion = Guid.Parse(value[0]);

            if (Context.User.Id != owner)
            {
                await Context.Interaction.RespondOrFollowupAsync("You are not the owner of this editor!", ephemeral: true);
                return;
            }

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
            var serverApplicationSettings = await Database.ServerApplicationSettings.FirstAsync(x => x.ServerId == server.Id);
            var application = await Database.ServerApplications.Where(x => x.ApplicationSettingsId == serverApplicationSettings.Id).FirstOrDefaultAsync(x => x.Id == appId);

            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            var question = await Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).FirstOrDefaultAsync(x => x.Id == selectedQuestion);

            if(question == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The question with the id {selectedQuestion} does not exist!", ephemeral: true);
                return;
            }

            Database.ServerApplicationQuestions.Remove(question);
            await Database.SaveChangesAsync();
            await Context.Interaction.RespondOrFollowupAsync($"Successfully removed question {question.Index} with id {question.Id}.");

            _ = ModifyOriginalResponseAsync(x => { x.Embed = Utils.CreateApplicationEditorEmbed(application, Context.User, pageN).Build(); x.Components = Utils.CreateApplicationEditorComponents(application, Context.User, pageN).Build(); }); //We don't care if it fails.
        }

        [ComponentInteraction($"{Constants.EDIT_APP_QUESTION_SELECTION}:*,*")]
        public async Task EditQuestion(string author, string applicationId, string[] value)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var selectedQuestion = Guid.Parse(value[0]);

            if (Context.User.Id != owner)
            {
                await Context.Interaction.RespondOrFollowupAsync("You are not the owner of this editor!", ephemeral: true);
                return;
            }

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
            var serverApplicationSettings = await Database.ServerApplicationSettings.FirstAsync(x => x.ServerId == server.Id);
            var application = await Database.ServerApplications.Where(x => x.ApplicationSettingsId == serverApplicationSettings.Id).FirstOrDefaultAsync(x => x.Id == appId);

            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            var question = await Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).FirstOrDefaultAsync(x => x.Id == selectedQuestion);

            if (question == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The question with the id {selectedQuestion} does not exist!", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<EditApplicationQuestionModal>($"{Constants.EDIT_APP_QUESTION_MODAL}:{owner},{appId},{selectedQuestion}", modifyModal: (modal) =>
            {
                modal.UpdateTextInput("question", question.Question);
                modal.UpdateTextInput("required", question.Required);
            });
        }
    }
}
