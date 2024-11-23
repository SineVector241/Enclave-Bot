using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;

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

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;
            
            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            var questions = Database.ServerApplicationQuestions.Count(x => x.ApplicationId == application.Id);
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

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;
            
            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            var applicationQuestions = Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).ToArray();
            var selectionMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.REMOVE_APP_QUESTION_SELECTION}:{owner},{Context.Interaction.Message.Id},{appId}")
                .WithMaxValues(applicationQuestions.Length);

            for (var i = pageN * Constants.ListLimit; i < applicationQuestions.Length && i < (pageN * Constants.ListLimit + Constants.ListLimit); i++)
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

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            var applicationQuestions = Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).ToArray();
            var selectionMenu = new SelectMenuBuilder()
                                .WithCustomId($"{Constants.EDIT_APP_QUESTION_SELECTION}:{owner},{Context.Interaction.Message.Id},{appId}");

            for (var i = pageN * Constants.ListLimit; i < applicationQuestions.Length && i < (pageN * Constants.ListLimit + Constants.ListLimit); i++)
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
            try
            {
                var owner = ulong.Parse(author);
                var appId = Guid.Parse(applicationId);

                if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

                var application = await Database.GetApplicationById(Context.Guild.Id, appId);
                if (application == null)
                {
                    await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                    return;
                }

                var embed = Utils.CreateApplicationEditorActionEmbed(application, Context.User);
                var components = Utils.CreateApplicationEditorActionComponents(application, Context.User);

                await Context.Interaction.DeferSafelyAsync();
                await ModifyOriginalResponseAsync(x =>
                {
                    x.Embed = embed.Build();
                    x.Components = components.Build();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [ComponentInteraction($"{Constants.APP_QUESTIONS_NEXT_PAGE}:*,*,*")]
        public async Task QuestionNextPage(string author, string applicationId, string page)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var pageN = int.Parse(page);

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
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

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
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

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }
            if (application.AddRoles.Count >= Constants.ApplicationAddRolesLimit)
            {
                await Context.Interaction.RespondOrFollowupAsync($"You have reached the application add role limit of {Constants.ApplicationAddRolesLimit}!", ephemeral: true);
                return;
            }

            var selectionMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.ADD_APP_ADDITION_ROLE_SELECTION}:{owner},{Context.Interaction.Message.Id},{appId}")
                .WithMaxValues(Constants.ApplicationAddRolesLimit - application.AddRoles.Count)
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

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
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
                .WithCustomId($"{Constants.REMOVE_APP_ADDITION_ROLE_SELECTION}:{owner},{Context.Interaction.Message.Id},{appId}")
                .WithMaxValues(Constants.ApplicationAddRolesLimit)
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

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }
            if (application.RemoveRoles.Count >= Constants.ApplicationRemoveRolesLimit)
            {
                await Context.Interaction.RespondOrFollowupAsync($"You have reached the application remove role limit of {Constants.ApplicationAddRolesLimit}!", ephemeral: true);
                return;
            }

            var selectionMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.ADD_APP_REMOVAL_ROLE_SELECTION}:{owner},{Context.Interaction.Message.Id},{appId}")
                .WithMaxValues(Constants.ApplicationAddRolesLimit - application.RemoveRoles.Count)
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

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
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
                .WithCustomId($"{Constants.REMOVE_APP_REMOVAL_ROLE_SELECTION}:{owner},{Context.Interaction.Message.Id},{appId}")
                .WithMaxValues(Constants.ApplicationAddRolesLimit)
                .WithType(ComponentType.RoleSelect);
            var components = new ComponentBuilder()
                .WithSelectMenu(selectionMenu);

            await Context.Interaction.RespondOrFollowupAsync(components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction($"{Constants.SET_APP_ACCEPT_MESSAGE}:*,*")]
        public async Task SetAcceptMessage(string author, string applicationId)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<ApplicationMessageModal>($"{Constants.SET_APP_ACCEPT_MESSAGE_MODAL}:{Context.Interaction.Message.Id},{appId}", modifyModal: (modal) =>
            {
                modal.UpdateTextInput("message", application.AcceptMessage);
            });
        }

        [ComponentInteraction($"{Constants.SET_APP_SUBMISSION_CHANNEL}:*,*")]
        public async Task SetSubmissionChannel(string author, string applicationId)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            var selectionMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.SET_APP_SUBMISSION_CHANNEL_SELECTION}:{owner},{Context.Interaction.Message.Id},{appId}")
                .WithType(ComponentType.ChannelSelect)
                .WithChannelTypes(ChannelType.Text);
            var components = new ComponentBuilder()
                .WithSelectMenu(selectionMenu);

            await Context.Interaction.RespondOrFollowupAsync(components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction($"{Constants.SET_APP_RETRIES}:*,*")]
        public async Task SetRetries(string author, string applicationId)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<ApplicationRetriesModal>($"{Constants.SET_APP_RETRIES_MODAL}:{Context.Interaction.Message.Id},{appId}", modifyModal: (modal) =>
            {
                modal.UpdateTextInput("retries", application.Retries.ToString());
            });
        }

        [ComponentInteraction($"{Constants.SET_APP_FAIL_ACTION}:*,*")]
        public async Task SetDenyAction(string author, string applicationId)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            var selectionMenu = new SelectMenuBuilder()
                .WithCustomId($"{Constants.SET_APP_FAIL_ACTION_SELECTION}:{owner},{Context.Interaction.Message.Id},{appId}");
            

            foreach (int i in Enum.GetValues(typeof(ApplicationFailAction)))
            {
                selectionMenu.AddOption(Enum.GetName(typeof(ApplicationFailAction), i), i.ToString());
            }
            
            var components = new ComponentBuilder()
                .WithSelectMenu(selectionMenu);
            
            await Context.Interaction.RespondOrFollowupAsync(components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction($"{Constants.SWITCH_TO_APP_QUESTIONS}:*,*")]
        public async Task SwitchToQuestions(string author, string applicationId)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
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