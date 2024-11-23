using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;

namespace Enclave_Bot.Core.Applications
{
    public class Modals(DatabaseContext database, Utils utils) : InteractionModuleBase<SocketInteractionContext<SocketModal>>
    {
        private readonly DatabaseContext Database = database;
        private readonly Utils Utils = utils;

        [ModalInteraction($"{Constants.ADD_APP_QUESTION_MODAL}:*,*")]
        public async Task AddQuestion(string originalMessage, string applicationId, ApplicationQuestionModal modal)
        {
            var editorId = ulong.Parse(originalMessage);
            var appId = Guid.Parse(applicationId);
            var editor = (IUserMessage)(Context.Channel.GetCachedMessage(editorId) ?? await Context.Channel.GetMessageAsync(editorId));

            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }
            if (!bool.TryParse(modal.Required, out var required))
            {
                await Context.Interaction.RespondOrFollowupAsync($"Invalid required value! Required value must be either True or False!", ephemeral: true);
                return;
            }

            var questions = Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).ToList();

            if (questions.Count > Constants.ApplicationQuestionsLimit)
            {
                await Context.Interaction.RespondOrFollowupAsync($"You have reached the application question limit of {Constants.ApplicationQuestionsLimit}.", ephemeral: true);
                return;
            }

            if (int.TryParse(modal.Index, out var index))
            {
                var question = new ApplicationQuestion() { ApplicationId = application.Id, Question = modal.Question.Truncate(Constants.ValueLimit) ?? string.Empty, Required = required, Index = index };
                await Database.ServerApplicationQuestions.AddAsync(question);
                while (questions.Exists(x => x.Index == index && x != question))
                {
                    question = questions.First(x => x.Index == index && x != question);
                    question.Index = index + 1;
                    index++;
                }
                await Database.SaveChangesAsync();
                await Context.Interaction.RespondOrFollowupAsync($"Successfully added question to index {index}.", ephemeral: true);
            }
            else
            {
                for (var i = 0; i < int.MaxValue; i++)
                {
                    if (questions.Exists(x => x.Index == i)) continue;
                    await Database.ServerApplicationQuestions.AddAsync(new ApplicationQuestion() { ApplicationId = application.Id, Question = modal.Question.Truncate(Constants.ValueLimit) ?? string.Empty, Required = required, Index = i });
                    await Database.SaveChangesAsync();
                    await Context.Interaction.RespondOrFollowupAsync($"Successfully added question to index {i}.", ephemeral: true);
                    break;
                }
            }

            _ = editor.ModifyAsync(x => { x.Embed = Utils.CreateApplicationEditorEmbed(application, Context.User).Build(); x.Components = Utils.CreateApplicationEditorComponents(application, Context.User).Build(); }); //We don't care if it fails.
        }

        [ModalInteraction($"{Constants.EDIT_APP_QUESTION_MODAL}:*,*,*")]
        public async Task EditQuestion(string originalMessage, string applicationId, string selectedQuestion, ApplicationQuestionModal modal)
        {
            var editorId = ulong.Parse(originalMessage);
            var appId = Guid.Parse(applicationId);
            var selectedQ = Guid.Parse(selectedQuestion);
            var editor = (IUserMessage)(Context.Channel.GetCachedMessage(editorId) ?? await Context.Channel.GetMessageAsync(editorId));
            
            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }
            if (!bool.TryParse(modal.Required, out var required))
            {
                await Context.Interaction.RespondOrFollowupAsync($"Invalid required value! Required value must be either True or False!", ephemeral: true);
                return;
            }

            var questions = Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id).ToList();
            var question = questions.FirstOrDefault(x => x.Id == selectedQ);

            if (question == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The question with the id {selectedQ} does not exist!", ephemeral: true);
                return;
            }

            question.Question = modal.Question.Truncate(Constants.ValueLimit) ?? string.Empty;
            question.Required = required;

            if (int.TryParse(modal.Index, out var index))
            {
                question.Index = index;
                while (questions.Exists(x => x.Index == index && x != question))
                {
                    question = questions.First(x => x.Index == index && x != question);
                    question.Index = index + 1;
                    index++;
                }
            }

            await Database.SaveChangesAsync();
            await Context.Interaction.DeferSafelyAsync();
            _ = ModifyOriginalResponseAsync(x => { x.Content = $"Question with id {selectedQ} was successfully edited!"; x.Components = null; });
            _ = editor.ModifyAsync(x => { x.Embed = Utils.CreateApplicationEditorEmbed(application, Context.User).Build(); x.Components = Utils.CreateApplicationEditorComponents(application, Context.User).Build(); }); //We don't care if it fails.
        }

        [ModalInteraction($"{Constants.SET_APP_ACCEPT_MESSAGE_MODAL}:*,*")]
        public async Task SetAcceptMessage(string originalMessage, string applicationId, ApplicationMessageModal modal)
        {
            var editorId = ulong.Parse(originalMessage);
            var appId = Guid.Parse(applicationId);
            var editor = (IUserMessage)(Context.Channel.GetCachedMessage(editorId) ?? await Context.Channel.GetMessageAsync(editorId));
            
            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }
            
            application.AcceptMessage = modal.Message.Truncate(Constants.ValueLimit) ?? string.Empty;
            await Database.SaveChangesAsync();
            await Context.Interaction.RespondOrFollowupAsync("Application accept message was set!", ephemeral: true);
            _ = editor.ModifyAsync(x => { x.Embed = Utils.CreateApplicationEditorActionEmbed(application, Context.User).Build(); x.Components = Utils.CreateApplicationEditorActionComponents(application, Context.User).Build(); }); //We don't care if it fails.
        }
        
        [ModalInteraction($"{Constants.SET_APP_RETRIES_MODAL}:*,*")]
        public async Task SetAcceptMessage(string originalMessage, string applicationId, ApplicationRetriesModal modal)
        {
            var editorId = ulong.Parse(originalMessage);
            var appId = Guid.Parse(applicationId);
            var editor = (IUserMessage)(Context.Channel.GetCachedMessage(editorId) ?? await Context.Channel.GetMessageAsync(editorId));
            
            var application = await Database.GetApplicationById(Context.Guild.Id, appId);
            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The application with the id {appId} does not exist!", ephemeral: true);
                return;
            }

            if (!byte.TryParse(modal.Retries, out var retries))
            {
                await Context.Interaction.RespondOrFollowupAsync($"Invalid required value! Required value must be between {byte.MinValue} and {byte.MaxValue}!", ephemeral: true);
                return;
            }

            application.Retries = retries;
            await Database.SaveChangesAsync();
            await Context.Interaction.RespondOrFollowupAsync($"Successfully set application retries to {retries}!", ephemeral: true);
            _ = editor.ModifyAsync(x => { x.Embed = Utils.CreateApplicationEditorActionEmbed(application, Context.User).Build(); x.Components = Utils.CreateApplicationEditorActionComponents(application, Context.User).Build(); }); //We don't care if it fails.
        }
    }

    public class ApplicationQuestionModal : IModal
    {
        public string Title => "Add Question";

        [InputLabel("Question")]
        [ModalTextInput("question", TextInputStyle.Paragraph, "Question...", maxLength: Constants.ValueLimit)]
        public string Question { get; set; } = string.Empty;

        [InputLabel("Required?")]
        [ModalTextInput("required", TextInputStyle.Short, "true", maxLength: 5, initValue: "True")]
        public string Required { get; set; } = true.ToString();

        [InputLabel("Index")]
        [ModalTextInput("index", TextInputStyle.Short, "0", maxLength: 2, initValue: "0")]
        public string Index { get; set; } = string.Empty;
    }

    public class ApplicationMessageModal : IModal
    {
        public string Title => "Set Message";
        
        [InputLabel("Message")]
        [ModalTextInput("message", TextInputStyle.Paragraph, "Message...", maxLength: Constants.DescriptionLimit)]
        public string Message { get; set; } = string.Empty;
    }

    public class ApplicationRetriesModal : IModal
    {
        public string Title => "Set Retries";
        
        [InputLabel("Retries")]
        [ModalTextInput("retries", TextInputStyle.Short, "0", maxLength: 3, initValue: "0")]
        public string Retries { get; set; } = string.Empty;
    }
}
