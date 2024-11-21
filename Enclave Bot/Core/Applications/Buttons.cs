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

        #region App Questions
        [ComponentInteraction($"{Constants.ADD_APP_QUESTION}:*,*")]
        public async Task AddQuestion(string author, string applicationId)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);

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

            var questions = Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).Count();
            if (questions > Constants.ApplicationQuestionsLimit)
            {
                await Context.Interaction.RespondOrFollowupAsync($"You have reached the application question limit of {Constants.ApplicationQuestionsLimit}.", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<ApplicationQuestionModal>($"{Constants.ADD_APP_QUESTION_MODAL}:{Context.Interaction.Message.Id},{appId}");
        }

        [ComponentInteraction($"{Constants.REMOVE_APP_QUESTION}:*,*,*")]
        public async Task RemoveQuestion(string author, string applicationId, string page)
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

            var applicationQuestions = Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).ToArray();
            var selectionMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.REMOVE_APP_QUESTION_SELECTION}:{owner},{Context.Interaction.Message.Id},{appId}")
                .WithMaxValues(applicationQuestions.Length);

            for (int i = pageN * Constants.ListLimit; i < applicationQuestions.Length && i < (pageN * Constants.ListLimit + Constants.ListLimit); i++)
            {
                selectionMenu.AddOption(i.ToString(), applicationQuestions[i].Id.ToString(), applicationQuestions[i].Question.Truncate(100));
            }

            if(selectionMenu.Options.Count <= 0)
            {
                await Context.Interaction.RespondOrFollowupAsync($"There are no questions to remove!", ephemeral: true);
                return;
            }

            var components = new ComponentBuilder()
                .WithSelectMenu(selectionMenu);
            await Context.Interaction.RespondOrFollowupAsync(components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction($"{Constants.EDIT_APP_QUESTION}:*,*,*")]
        public async Task EditQuestion(string author, string applicationId, string page)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var pageN = int.Parse(page);

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

            var applicationQuestions = Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).ToArray();
            var selectionMenu = new SelectMenuBuilder()
                                .WithCustomId($"{Constants.EDIT_APP_QUESTION_SELECTION}:{owner},{Context.Interaction.Message.Id},{appId}");

            for (int i = pageN * Constants.ListLimit; i < applicationQuestions.Length && i < (pageN * Constants.ListLimit + Constants.ListLimit); i++)
            {
                selectionMenu.AddOption(i.ToString(), applicationQuestions[i].Id.ToString(), applicationQuestions[i].Question.Truncate(Constants.ValueLimit));
            }

            var components = new ComponentBuilder()
                .WithSelectMenu(selectionMenu);
            await Context.Interaction.RespondOrFollowupAsync(components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction($"{Constants.SWITCH_TO_APP_ACTIONS}:*,*")]
        public async Task SwitchToActions(string author, string applicationId)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);

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

            var embed = Utils.CreateApplicationEditorActionEmbed(application, Context.User);
            var components = Utils.CreateApplicationEditorActionComponents(application, Context.User);

            await Context.Interaction.DeferSafelyAsync();
            await ModifyOriginalResponseAsync(x => { x.Embed = embed.Build(); x.Components = components.Build(); });
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
            await ModifyOriginalResponseAsync(x => { x.Embed = Utils.CreateApplicationEditorEmbed(application, Context.User, pageN + 1).Build(); x.Components = Utils.CreateApplicationEditorComponents(application, Context.User, pageN + 1).Build(); }); //We don't care if it fails.
        }

        [ComponentInteraction($"{Constants.APP_QUESTIONS_PREVIOUS_PAGE}:*,*,*")]
        public async Task QuestionPreviousPage(string author, string applicationId, string page)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var pageN = int.Parse(page);

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

            await Context.Interaction.DeferSafelyAsync();
            await ModifyOriginalResponseAsync(x => { x.Embed = Utils.CreateApplicationEditorEmbed(application, Context.User, pageN - 1).Build(); x.Components = Utils.CreateApplicationEditorComponents(application, Context.User, pageN - 1).Build(); }); //We don't care if it fails.
        }
        #endregion

        #region App Actions
        [ComponentInteraction($"{Constants.ADD_APP_ADDITION_ROLE}:*,*")]
        public async Task AddAdditionRole(string author, string applicationId)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);

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
            if (application.AddRoles.Count > Constants.ApplicationAddRolesLimit)
            {
                await Context.Interaction.RespondOrFollowupAsync($"You have reached the application add role limit of {Constants.ApplicationAddRolesLimit}.", ephemeral: true);
                return;
            }

            var selectionMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.ADD_APP_ADDTION_ROLE_SELECTION}:{owner},{appId}")
                .WithType(ComponentType.RoleSelect);
            var components = new ComponentBuilder()
                .WithSelectMenu(selectionMenu);

            await Context.Interaction.RespondOrFollowupAsync(components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction($"{Constants.REMOVE_APP_ADDITION_ROLE}:*,*")]
        public async Task RemoveAdditionRole(string author, string applicationId)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);

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
            if (application.AddRoles.Count <= 0)
            {
                await Context.Interaction.RespondOrFollowupAsync($"There are no roles to remove!", ephemeral: true);
                return;
            }

            var selectionMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.REMOVE_APP_ADDTION_ROLE_SELECTION}:{owner},{appId}")
                .WithType(ComponentType.RoleSelect);
            var components = new ComponentBuilder()
                .WithSelectMenu(selectionMenu);

            await Context.Interaction.RespondOrFollowupAsync(components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction($"{Constants.ADD_APP_REMOVAL_ROLE}:*,*")]
        public async Task AddRemovalRole(string author, string applicationId)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);

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
            if (application.RemoveRoles.Count > Constants.ApplicationRemoveRolesLimit)
            {
                await Context.Interaction.RespondOrFollowupAsync($"You have reached the application remove role limit of {Constants.ApplicationAddRolesLimit}.", ephemeral: true);
                return;
            }

            var selectionMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.ADD_APP_REMOVAL_ROLE_SELECTION}:{owner},{appId}")
                .WithType(ComponentType.RoleSelect);
            var components = new ComponentBuilder()
                .WithSelectMenu(selectionMenu);

            await Context.Interaction.RespondOrFollowupAsync(components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction($"{Constants.REMOVE_APP_REMOVAL_ROLE}:*,*")]
        public async Task RemoveRemovalRole(string author, string applicationId)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);

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
            if (application.RemoveRoles.Count <= 0)
            {
                await Context.Interaction.RespondOrFollowupAsync($"There are no roles to remove!", ephemeral: true);
                return;
            }

            var selectionMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.REMOVE_APP_REMOVAL_ROLE_SELECTION}:{owner},{appId}")
                .WithType(ComponentType.RoleSelect);
            var components = new ComponentBuilder()
                .WithSelectMenu(selectionMenu);

            await Context.Interaction.RespondOrFollowupAsync(components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction($"{Constants.SET_APP_ACCEPT_MESSAGE}:*,*")]
        public async Task SetAcceptMessage(string author, string applicationId)
        {
            await Context.Interaction.RespondOrFollowupAsync("TODO!");
        }

        [ComponentInteraction($"{Constants.SET_APP_SUBMISSION_CHANNEL}:*,*")]
        public async Task SetSubmissionChannel(string author, string applicationId)
        {
            await Context.Interaction.RespondOrFollowupAsync("TODO!");
        }

        [ComponentInteraction($"{Constants.SET_APP_RETRIES}:*,*")]
        public async Task SetRetries(string author, string applicationId)
        {
            await Context.Interaction.RespondOrFollowupAsync("TODO!");
        }

        [ComponentInteraction($"{Constants.SET_APP_DENY_ACTION}:*,*")]
        public async Task SetDenyAction(string author, string applicationId)
        {
            await Context.Interaction.RespondOrFollowupAsync("TODO!");
        }

        [ComponentInteraction($"{Constants.SWITCH_TO_APP_QUESTIONS}:*,*")]
        public async Task SwitchToQuestions(string author, string applicationId)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);

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

            var embed = Utils.CreateApplicationEditorEmbed(application, Context.User);
            var components = Utils.CreateApplicationEditorComponents(application, Context.User);

            await Context.Interaction.DeferSafelyAsync();
            await ModifyOriginalResponseAsync(x => { x.Embed = embed.Build(); x.Components = components.Build(); });
        }
        #endregion
    }
}