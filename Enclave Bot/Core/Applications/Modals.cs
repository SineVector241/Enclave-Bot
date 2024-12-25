using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Preconditions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Applications
{
    [IsStaff]
    public class Modals(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketModal>>
    {
        [ModalInteraction($"{Constants.APPLICATION_MODAL_CREATE}")]
        public async Task CreateApplication(CreateApplicationModal modal)
        {
            if (modal.Name.Length > Constants.APPLICATION_TITLE_CHARACTER_LIMIT)
            {
                await RespondAsync($"Name can only be {Constants.APPLICATION_TITLE_CHARACTER_LIMIT} characters!", ephemeral: true);
                return;
            }

            var applicationSettings = (await database.Servers
                .Include(x => x.ApplicationSettings)
                .ThenInclude(x => x.Applications)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ApplicationSettings;
            var applications = applicationSettings.Applications;
            
            applications.Add(new Application { ApplicationSettings = applicationSettings, Name = modal.Name });
            await database.SaveChangesAsync();
            await RespondAsync($"Created application {modal.Name}.", ephemeral: true);
            
            //Don't really care if it fails.
            var applicationList = Utils.CreateApplicationList(applications.ToArray(), 0, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationList.Item1;
                x.Components = applicationList.Item2;
            });
        }

        [ModalInteraction($"{Constants.APPLICATION_MODAL_CREATE_QUESTION}:*")]
        public async Task CreateApplicationQuestion(string sApplicationId, ApplicationQuestionModal modal)
        {
            var applicationId = long.Parse(sApplicationId);
            
            if (modal.Question.Length > Constants.APPLICATION_QUESTION_CHARACTER_LIMIT)
            {
                await RespondAsync($"Question can only be {Constants.APPLICATION_QUESTION_CHARACTER_LIMIT} characters!", ephemeral: true);
                return;
            }
            if (!bool.TryParse(modal.Required, out var required))
            {
                await RespondAsync($"Invalid required value! Required value must be either True or False!", ephemeral: true);
                return;
            }

            var applications = (await database.Servers
                .Include(x => x.ApplicationSettings)
                .ThenInclude(x => x.Applications)
                .ThenInclude(x => x.Questions)
                .FirstAsync(x => x.Id == Context.Guild.Id)).ApplicationSettings.Applications;
            var application = applications.FirstOrDefault(x => x.Id == applicationId);
            
            if (application == null)
            {
                await RespondAsync($"Application with id {applicationId} was not found!", ephemeral: true);
                return;
            }
            if (int.TryParse(modal.Index, out var index) && index >= 0)
            {
                var question = new ApplicationQuestion { Application = application, Question = modal.Question, Required = required, Index = index };
                application.Questions.Add(question);
                while (application.Questions.Exists(x => x.Index == index && x != question))
                {
                    question = application.Questions.First(x => x.Index == index && x != question);
                    question.Index = index + 1;
                    index++;
                }
            }
            else
            {
                for (var i = 0; i < int.MaxValue; i++)
                {
                    if (application.Questions.Exists(x => x.Index == i)) continue;
                    application.Questions.Add(new ApplicationQuestion { Application = application, Question = modal.Question, Required = required, Index = i });
                    index = i;
                    break;
                }
            }
            
            await database.SaveChangesAsync();
            await RespondAsync($"Successfully added question to index {index}.", ephemeral: true);
            
            //Don't really care if it fails.
            var applicationEditor = Utils.CreateApplicationQuestionEditor(application, 0, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationEditor.Item1;
                x.Components = applicationEditor.Item2;
            });
        }

        [ModalInteraction($"{Constants.APPLICATION_MODAL_EDIT_QUESTION}:*,*")]
        public async Task EditApplicationQuestion(string sApplicationId, string sQuestionId, ApplicationQuestionModal modal)
        {
            var applicationId = long.Parse(sApplicationId);
            var questionId = long.Parse(sQuestionId);

            if (modal.Question.Length > Constants.APPLICATION_QUESTION_CHARACTER_LIMIT)
            {
                await RespondAsync($"Question can only be {Constants.APPLICATION_QUESTION_CHARACTER_LIMIT} characters!", ephemeral: true);
                return;
            }
            if (!bool.TryParse(modal.Required, out var required))
            {
                await RespondAsync($"Invalid required value! Required value must be either True or False!", ephemeral: true);
                return;
            }

            var applications = (await database.Servers
                .Include(x => x.ApplicationSettings)
                .ThenInclude(x => x.Applications)
                .ThenInclude(x => x.Questions)
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

            question.Question = modal.Question;
            question.Required = required;
            if (int.TryParse(modal.Index, out var index) && index >= 0 && index != question.Index)
            {
                question.Index = index;
                while (application.Questions.Exists(x => x.Index == index && x != question))
                {
                    question = application.Questions.First(x => x.Index == index && x != question);
                    question.Index = index + 1;
                    index++;
                }
            }

            await database.SaveChangesAsync();
            await RespondAsync($"Successfully edited question with id {question.Id}.", ephemeral: true);

            //Don't really care if it fails.
            var applicationEditor = Utils.CreateApplicationQuestionEditor(application, 0, Context.User);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationEditor.Item1;
                x.Components = applicationEditor.Item2;
            });
        }

        [ModalInteraction($"{Constants.APPLICATION_MODAL_SET_RETRIES}:*")]
        public async Task SetFailAmount(string sApplicationId, ApplicationRetriesModal modal)
        {
            var applicationId = long.Parse(sApplicationId);
            if (!byte.TryParse(modal.Retries, out var retries))
            {
                await RespondAsync($"{modal.Retries} is not a valid number. The value must be between {byte.MinValue} and {byte.MaxValue}!", ephemeral: true);
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
            
            application.Retries = retries;
            await database.SaveChangesAsync();
            await RespondAsync($"Successfully set retries to {application.Retries}.", ephemeral: true);

            //Don't really care if it fails.
            var applicationEditor = Utils.CreateApplicationActionEditor(application, Context.User, Context.Guild);
            _ = Context.Interaction.Message.ModifyAsync(x =>
            {
                x.Embed = applicationEditor.Item1;
                x.Components = applicationEditor.Item2;
            });
        }
    }
    
    public class CreateApplicationModal : IModal
    {
        public string Title => "Create Application";

        [InputLabel("Name")]
        [ModalTextInput("name", maxLength: Constants.APPLICATION_TITLE_CHARACTER_LIMIT)]
        public string Name { get; set; } = string.Empty;
    }

    public class ApplicationQuestionModal : IModal
    {
        public string Title => "Create Question";
        
        [InputLabel("Question")]
        [ModalTextInput("question", TextInputStyle.Paragraph, maxLength: Constants.APPLICATION_QUESTION_CHARACTER_LIMIT)]
        public string Question { get; set; } = string.Empty;
        
        [InputLabel("Required")]
        [ModalTextInput("required", initValue: "True", placeholder: "True/False")]
        public string Required { get; set; } = string.Empty;
        
        [InputLabel("Index")]
        [ModalTextInput("index", initValue: "-1")]
        public string Index { get; set; } = string.Empty;
    }

    public class ApplicationRetriesModal : IModal
    {
        public string Title => "Set Retries";
        
        [InputLabel("Retries")]
        [ModalTextInput("retries", initValue: "-1")]
        public string Retries { get; set; } = string.Empty;
    }
}