using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Preconditions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Applications
{
    [IsStaff]
    public class SelectMenus(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction($"{Constants.APPLICATION_LIST_EDIT}:*")]
        public async Task EditApplication(string sAuthorId, string[] sApplicationId)
        {
            var authorId = ulong.Parse(sAuthorId);
            var applicationId = long.Parse(sApplicationId[0]);
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
                //Don't really care if it fails.
                var appList = Utils.CreateApplicationList(applications.ToArray(), 0, Context.User);
                _ = Context.Interaction.Message.ModifyAsync(x =>
                {
                    x.Embed = appList.Item1;
                    x.Components = appList.Item2;
                });
                await RespondAsync($"Application with id {applicationId} was not found!", ephemeral: true);
                return;
            }

            var applicationEditor = Utils.CreateApplicationQuestionEditor(application, 0, Context.User);
            await DeferAsync();
            await ModifyOriginalResponseAsync(x =>
            {
                x.Embed = applicationEditor.Item1;
                x.Components = applicationEditor.Item2;
            });
        }

        [ComponentInteraction($"{Constants.APPLICATION_LIST_DELETE}:*")]
        public async Task DeleteApplication(string sAuthorId, string[] sApplicationId)
        {
            var authorId = ulong.Parse(sAuthorId);
            var applicationId = long.Parse(sApplicationId[0]);
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
                //Don't really care if it fails.
                var appList = Utils.CreateApplicationList(applications.ToArray(), 0, Context.User);
                _ = Context.Interaction.Message.ModifyAsync(x =>
                {
                    x.Embed = appList.Item1;
                    x.Components = appList.Item2;
                });
                await RespondAsync($"Application with id {applicationId} was not found!", ephemeral: true);
                return;
            }

            database.Remove(application);
            await database.SaveChangesAsync();
            await RespondAsync($"Application with id {applicationId} was deleted!", ephemeral: true);

            //Don't really care if it fails.
            var applicationList = Utils.CreateApplicationList(applications.ToArray(), 0, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationList.Item1;
                x.Components = applicationList.Item2;
            });
        }

        [ComponentInteraction($"{Constants.APPLICATION_EDIT_EDIT_QUESTION}:*,*")]
        public async Task EditApplicationQuestion(string sAuthorId, string sApplicationId, string[] sQuestionId)
        {
            var authorId = ulong.Parse(sAuthorId);
            var applicationId = long.Parse(sApplicationId);
            var questionId = long.Parse(sQuestionId[0]);
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

            var question = application.Questions.FirstOrDefault(x => x.Id == questionId);
            if (question == null)
            {
                await RespondAsync($"Question with id {questionId} was not found!", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<ApplicationQuestionModal>($"{Constants.APPLICATION_MODAL_EDIT_QUESTION}:{applicationId},{questionId}",
                modifyModal: modal =>
                {
                    modal.WithTitle("Edit Question");
                    modal.UpdateTextInput("question", question.Question);
                    modal.UpdateTextInput("required", question.Required);
                    modal.UpdateTextInput("index", question.Index);
                }
            );
        }
        
        [ComponentInteraction($"{Constants.APPLICATION_EDIT_DELETE_QUESTION}:*,*")]
        public async Task DeleteApplicationQuestion(string sAuthorId, string sApplicationId, string[] sQuestionId)
        {
            var authorId = ulong.Parse(sAuthorId);
            var applicationId = long.Parse(sApplicationId);
            var questionId = long.Parse(sQuestionId[0]);
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

            var question = application.Questions.FirstOrDefault(x => x.Id == questionId);
            if (question == null)
            {
                await RespondAsync($"Question with id {questionId} was not found!", ephemeral: true);
                return;
            }
            
            application.Questions.Remove(question);
            await database.SaveChangesAsync();
            await RespondAsync($"Question with id {questionId} was deleted!", ephemeral: true);
            
            //Don't really care if it fails.
            var applicationEditor = Utils.CreateApplicationQuestionEditor(application, 0, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationEditor.Item1;
                x.Components = applicationEditor.Item2;
            });
        }
        
        [ComponentInteraction($"{Constants.APPLICATION_EDIT_SET_SUBMISSION_CHANNEL}:*,*")]
        public async Task SetSubmissionChannel(string sAuthorId, string sApplicationId, ITextChannel[] channel)
        {
            var authorId = ulong.Parse(sAuthorId);
            var applicationId = long.Parse(sApplicationId);
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
            
            application.SubmissionChannel = channel.FirstOrDefault()?.Id;
            await database.SaveChangesAsync();
            await RespondAsync($"Successfully set submission channel!", ephemeral: true);
            
            //Don't really care if it fails.
            var applicationEditor = Utils.CreateApplicationActionEditor(application, Context.User, Context.Guild);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationEditor.Item1;
                x.Components = applicationEditor.Item2;
            });
        }

        [ComponentInteraction($"{Constants.APPLICATION_EDIT_SET_ADD_ROLES}:*,*")]
        public async Task SetAddRoles(string sAuthorId, string sApplicationId, IRole[] roles)
        {
            var authorId = ulong.Parse(sAuthorId);
            var applicationId = long.Parse(sApplicationId);
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

            application.AddRoles = roles.Select(x => x.Id).ToList();
            await database.SaveChangesAsync();
            await RespondAsync($"Successfully set add roles!", ephemeral: true);
            
            //Don't really care if it fails.
            var applicationEditor = Utils.CreateApplicationActionEditor(application, Context.User, Context.Guild);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationEditor.Item1;
                x.Components = applicationEditor.Item2;
            });
        }
        
        [ComponentInteraction($"{Constants.APPLICATION_EDIT_SET_REMOVE_ROLES}:*,*")]
        public async Task SetRemoveRoles(string sAuthorId, string sApplicationId, IRole[] roles)
        {
            var authorId = ulong.Parse(sAuthorId);
            var applicationId = long.Parse(sApplicationId);
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

            application.RemoveRoles = roles.Select(x => x.Id).ToList();
            await database.SaveChangesAsync();
            await RespondAsync($"Successfully set remove roles!", ephemeral: true);
            
            //Don't really care if it fails.
            var applicationEditor = Utils.CreateApplicationActionEditor(application, Context.User, Context.Guild);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationEditor.Item1;
                x.Components = applicationEditor.Item2;
            });
        }
        
        [ComponentInteraction($"{Constants.APPLICATION_EDIT_SET_FAIL_MODE}:*,*")]
        public async Task SetFailMode(string sAuthorId, string sApplicationId, string[] sFailMode)
        {
            var authorId = ulong.Parse(sAuthorId);
            var applicationId = long.Parse(sApplicationId);
            var failMode = (ApplicationFailAction)int.Parse(sFailMode[0]);
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

            application.FailAction = failMode;
            await database.SaveChangesAsync();
            await RespondAsync($"Successfully set remove roles!", ephemeral: true);
            
            //Don't really care if it fails.
            var applicationEditor = Utils.CreateApplicationActionEditor(application, Context.User, Context.Guild);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationEditor.Item1;
                x.Components = applicationEditor.Item2;
            });
        }
    }
}