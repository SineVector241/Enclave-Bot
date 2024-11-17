using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Applications
{
    public class Buttons(DatabaseContext database, Utils utils) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        private readonly DatabaseContext Database = database;
        private readonly Utils Utils = utils;

        [ComponentInteraction($"{Constants.ADD_APP_QUESTION}:*,*,*")]
        public async Task AddQuestion(string author, string applicationId, string page)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var pageN = int.Parse(page);

            if (Context.User.Id != owner)
            {
                await Context.Interaction.RespondOrFollowupAsync("You are not the owner of this editor!");
                return;
            }

            await RespondWithModalAsync<AddApplicationQuestionModal>($"{Constants.ADD_APP_QUESTION_MODAL}:{owner},{appId},{pageN}");
        }

        [ComponentInteraction($"{Constants.REMOVE_APP_QUESTION}:*,*,*")]
        public async Task RemoveQuestion(string author, string applicationId, string page)
        {
            try
            {
                var owner = ulong.Parse(author);
                var appId = Guid.Parse(applicationId);
                var pageN = int.Parse(page);

                await Context.Interaction.DeferSafelyAsync();
                if (Context.User.Id != owner)
                {
                    await Context.Interaction.RespondOrFollowupAsync("You are not the owner of this editor!");
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

                var applicationQuestions = Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).ToArray();
                var selectionMenu = new SelectMenuBuilder()
                    .WithCustomId($"{Constants.REMOVE_APP_QUESTION_SELECTION}:{owner},{appId},{pageN}");

                for (int i = pageN * Utils.ListLimit; i < applicationQuestions.Length && i < (pageN * Utils.ListLimit + Utils.ListLimit); i++)
                {
                    selectionMenu.AddOption(i.ToString(), applicationQuestions[i].Id.ToString(), applicationQuestions[i].Question.Truncate(100));
                }

                var components = new ComponentBuilder()
                    .WithSelectMenu(selectionMenu);
                await Context.Interaction.RespondOrFollowupAsync(components: components.Build(), ephemeral: true);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [ComponentInteraction($"{Constants.APP_QUESTIONS_NEXT_PAGE}:*,*,*")]
        public async Task QuestionNextPage(string author, string applicationId, string page)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var pageN = int.Parse(page);

            if (Context.User.Id != owner)
            {
                await Context.Interaction.RespondOrFollowupAsync("You are not the owner of this editor!");
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

            await Context.Interaction.DeferSafelyAsync();
            _ = ModifyOriginalResponseAsync(x => { x.Embed = Utils.CreateApplicationEditorEmbed(application, Context.User, pageN + 1).Build(); x.Components = Utils.CreateApplicationEditorComponents(application, Context.User, pageN + 1).Build(); }); //We don't care if it fails.
        }

        [ComponentInteraction($"{Constants.APP_QUESTIONS_PREVIOUS_PAGE}:*,*,*")]
        public async Task QuestionPreviousPage(string author, string applicationId, string page)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var pageN = int.Parse(page);

            if (Context.User.Id != owner)
            {
                await Context.Interaction.RespondOrFollowupAsync("You are not the owner of this editor!");
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

            await Context.Interaction.DeferSafelyAsync();
            _ = ModifyOriginalResponseAsync(x => { x.Embed = Utils.CreateApplicationEditorEmbed(application, Context.User, pageN - 1).Build(); x.Components = Utils.CreateApplicationEditorComponents(application, Context.User, pageN - 1).Build(); }); //We don't care if it fails.
        }
    }
}