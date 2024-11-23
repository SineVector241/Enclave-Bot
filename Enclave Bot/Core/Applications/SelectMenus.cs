using Discord;
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

        #region App Questions

        [ComponentInteraction($"{Constants.REMOVE_APP_QUESTION_SELECTION}:*,*,*")]
        public async Task RemoveQuestion(string author, string originalMessage, string applicationId, string[] value)
        {
            var editorId = ulong.Parse(originalMessage);
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var selectedQuestions = value.Select(Guid.Parse);
            var editor = (IUserMessage)(Context.Channel.GetCachedMessage(editorId) ?? await Context.Channel.GetMessageAsync(editorId));

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            var removed = await Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id && selectedQuestions.Contains(x.Id)).ExecuteDeleteAsync();
            if (removed <= 0)
            {
                await Context.Interaction.RespondOrFollowupAsync("No questions were removed!", ephemeral: true);
                return;
            }

            await Context.Interaction.DeferSafelyAsync();
            await Database.SaveChangesAsync();
            _ = ModifyOriginalResponseAsync(x =>
            {
                x.Content = $"Successfully removed {removed} question(s).";
                x.Components = null;
            });
            _ = editor.ModifyAsync(x =>
            {
                x.Embed = Utils.CreateApplicationEditorEmbed(application, Context.User).Build();
                x.Components = Utils.CreateApplicationEditorComponents(application, Context.User).Build();
            }); //We don't care if it fails.
        }

        [ComponentInteraction($"{Constants.EDIT_APP_QUESTION_SELECTION}:*,*,*")]
        public async Task EditQuestion(string author, string originalMessage, string applicationId, string[] value)
        {
            var editorId = ulong.Parse(originalMessage);
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var selectedQuestion = Guid.Parse(value[0]);

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
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

            await RespondWithModalAsync<ApplicationQuestionModal>($"{Constants.EDIT_APP_QUESTION_MODAL}:{editorId},{appId},{selectedQuestion}", modifyModal: (modal) =>
            {
                modal.WithTitle("Edit Question".Truncate(Constants.TitleLimit));
                modal.UpdateTextInput("question", question.Question);
                modal.UpdateTextInput("required", question.Required);
                modal.UpdateTextInput("index", question.Index);
            });
        }

        #endregion

        #region App Actions

        [ComponentInteraction($"{Constants.ADD_APP_ADDITION_ROLE_SELECTION}:*,*,*")]
        public async Task AddAdditionRole(string author, string originalMessage, string applicationId, SocketRole[] roles)
        {
            var editorId = ulong.Parse(originalMessage);
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var editor = (IUserMessage)(Context.Channel.GetCachedMessage(editorId) ?? await Context.Channel.GetMessageAsync(editorId));

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            var addedRoles = 0;
            foreach (var role in roles)
            {
                if (application.AddRoles.Count >= Constants.ApplicationAddRolesLimit)
                    break;
                if (application.AddRoles.Contains(role.Id))
                    continue;

                application.AddRoles.Add(role.Id);
                addedRoles++;
            }

            await Context.Interaction.DeferSafelyAsync();
            await Database.SaveChangesAsync();
            if (addedRoles <= 0)
            {
                _ = ModifyOriginalResponseAsync(x =>
                {
                    x.Content = "No roles were added!";
                    x.Components = null;
                });
                return;
            }

            _ = ModifyOriginalResponseAsync(x =>
            {
                x.Content = $"Successfully added {addedRoles} role(s).";
                x.Components = null;
            });
            _ = editor.ModifyAsync(x =>
            {
                x.Embed = Utils.CreateApplicationEditorActionEmbed(application, Context.User).Build();
                x.Components = Utils.CreateApplicationEditorActionComponents(application, Context.User).Build();
            }); //We don't care if it fails.
        }

        [ComponentInteraction($"{Constants.REMOVE_APP_ADDITION_ROLE_SELECTION}:*,*,*")]
        public async Task RemoveAdditionRole(string author, string originalMessage, string applicationId, SocketRole[] roles)
        {
            var editorId = ulong.Parse(originalMessage);
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var editor = (IUserMessage)(Context.Channel.GetCachedMessage(editorId) ?? await Context.Channel.GetMessageAsync(editorId));

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            var removedRoles = roles.TakeWhile(_ => application.AddRoles.Count > 0).Count(role => application.AddRoles.Remove(role.Id));
            await Context.Interaction.DeferSafelyAsync();
            await Database.SaveChangesAsync();
            if (removedRoles <= 0)
            {
                _ = ModifyOriginalResponseAsync(x =>
                {
                    x.Content = "No roles were removed!";
                    x.Components = null;
                });
                return;
            }

            _ = ModifyOriginalResponseAsync(x =>
            {
                x.Content = $"Successfully removed {removedRoles} role(s).";
                x.Components = null;
            });
            _ = editor.ModifyAsync(x =>
            {
                x.Embed = Utils.CreateApplicationEditorActionEmbed(application, Context.User).Build();
                x.Components = Utils.CreateApplicationEditorActionComponents(application, Context.User).Build();
            }); //We don't care if it fails.
        }
        
        [ComponentInteraction($"{Constants.ADD_APP_REMOVAL_ROLE_SELECTION}:*,*,*")]
        public async Task AddRemovalRole(string author, string originalMessage, string applicationId, SocketRole[] roles)
        {
            var editorId = ulong.Parse(originalMessage);
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var editor = (IUserMessage)(Context.Channel.GetCachedMessage(editorId) ?? await Context.Channel.GetMessageAsync(editorId));

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            var addedRoles = 0;
            foreach (var role in roles)
            {
                if (application.RemoveRoles.Count >= Constants.ApplicationRemoveRolesLimit)
                    break;
                if (application.RemoveRoles.Contains(role.Id))
                    continue;

                application.RemoveRoles.Add(role.Id);
                addedRoles++;
            }

            await Context.Interaction.DeferSafelyAsync();
            await Database.SaveChangesAsync();
            if (addedRoles <= 0)
            {
                _ = ModifyOriginalResponseAsync(x =>
                {
                    x.Content = "No roles were added!";
                    x.Components = null;
                });
                return;
            }

            _ = ModifyOriginalResponseAsync(x =>
            {
                x.Content = $"Successfully added {addedRoles} role(s).";
                x.Components = null;
            });
            _ = editor.ModifyAsync(x =>
            {
                x.Embed = Utils.CreateApplicationEditorActionEmbed(application, Context.User).Build();
                x.Components = Utils.CreateApplicationEditorActionComponents(application, Context.User).Build();
            }); //We don't care if it fails.
        }

        [ComponentInteraction($"{Constants.REMOVE_APP_REMOVAL_ROLE_SELECTION}:*,*,*")]
        public async Task RemoveRemovalRole(string author, string originalMessage, string applicationId, SocketRole[] roles)
        {
            var editorId = ulong.Parse(originalMessage);
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var editor = (IUserMessage)(Context.Channel.GetCachedMessage(editorId) ?? await Context.Channel.GetMessageAsync(editorId));

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            var removedRoles = roles.TakeWhile(_ => application.RemoveRoles.Count > 0).Count(role => application.RemoveRoles.Remove(role.Id));
            await Context.Interaction.DeferSafelyAsync();
            await Database.SaveChangesAsync();
            if (removedRoles <= 0)
            {
                _ = ModifyOriginalResponseAsync(x =>
                {
                    x.Content = "No roles were removed!";
                    x.Components = null;
                });
                return;
            }

            _ = ModifyOriginalResponseAsync(x =>
            {
                x.Content = $"Successfully removed {removedRoles} role(s).";
                x.Components = null;
            });
            _ = editor.ModifyAsync(x =>
            {
                x.Embed = Utils.CreateApplicationEditorActionEmbed(application, Context.User).Build();
                x.Components = Utils.CreateApplicationEditorActionComponents(application, Context.User).Build();
            }); //We don't care if it fails.
        }
        
        [ComponentInteraction($"{Constants.SET_APP_SUBMISSION_CHANNEL_SELECTION}:*,*,*")]
        public async Task SetSubmissionChannel(string author, string originalMessage, string applicationId, SocketChannel[] channels)
        {
            var editorId = ulong.Parse(originalMessage);
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var editor = (IUserMessage)(Context.Channel.GetCachedMessage(editorId) ?? await Context.Channel.GetMessageAsync(editorId));

            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            application.SubmissionChannel = channels[0].Id;
            await Context.Interaction.DeferSafelyAsync();
            await Database.SaveChangesAsync();
            _ = ModifyOriginalResponseAsync(x =>
            {
                x.Content = "Successfully set submission channel!";
                x.Components = null;
            });
            _ = editor.ModifyAsync(x =>
            {
                x.Embed = Utils.CreateApplicationEditorActionEmbed(application, Context.User).Build();
                x.Components = Utils.CreateApplicationEditorActionComponents(application, Context.User).Build();
            }); //We don't care if it fails.
        }

        [ComponentInteraction($"{Constants.SET_APP_FAIL_ACTION_SELECTION}:*,*,*")]
        public async Task SetFailAction(string author, string originalMessage, string applicationId, string[] failActions)
        {
            var editorId = ulong.Parse(originalMessage);
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);
            var editor = (IUserMessage)(Context.Channel.GetCachedMessage(editorId) ?? await Context.Channel.GetMessageAsync(editorId));
            var failAction = int.Parse(failActions[0]);
            
            if (!await Context.Interaction.CheckAuthorAsync(owner, "You are not the owner of this editor!", ephemeral: true)) return;

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            application.FailAction = (ApplicationFailAction)failAction;
            await Context.Interaction.DeferSafelyAsync();
            await Database.SaveChangesAsync();
            _ = ModifyOriginalResponseAsync(x =>
            {
                x.Content = $"Successfully set fail action to {application.FailAction}!";
                x.Components = null;
            });
            _ = editor.ModifyAsync(x =>
            {
                x.Embed = Utils.CreateApplicationEditorActionEmbed(application, Context.User).Build();
                x.Components = Utils.CreateApplicationEditorActionComponents(application, Context.User).Build();
            }); //We don't care if it fails.
        }
        #endregion
    }
}