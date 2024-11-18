﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Applications
{
    public class Modals(DatabaseContext database, Utils utils) : InteractionModuleBase<SocketInteractionContext<SocketModal>>
    {
        private readonly DatabaseContext Database = database;
        private readonly Utils Utils = utils;

        [ModalInteraction($"{Constants.ADD_APP_QUESTION_MODAL}:*,*")]
        public async Task AddQuestion(string originalMessage, string applicationId, AddApplicationQuestionModal modal)
        {
            var editorId = ulong.Parse(originalMessage);
            var appId = Guid.Parse(applicationId);
            var editor = (IUserMessage)(Context.Channel.GetCachedMessage(editorId) ?? await Context.Channel.GetMessageAsync(editorId));

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
            var serverApplicationSettings = await Database.ServerApplicationSettings.FirstAsync(x => x.ServerId == server.Id);
            var application = await Database.ServerApplications.Where(x => x.ApplicationSettingsId == serverApplicationSettings.Id).FirstOrDefaultAsync(x => x.Id == appId);

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

            for (int i = 0; i < int.MaxValue; i++)
            {
                if (!questions.Exists(x => x.Index == i))
                {
                    await Database.ServerApplicationQuestions.AddAsync(new ApplicationQuestion() { ApplicationId = application.Id, Question = modal.Question, Required = required, Index = i });
                    await Database.SaveChangesAsync();
                    await Context.Interaction.RespondOrFollowupAsync("Sucessfully added question.", ephemeral: true);
                    break;
                }
            }

            _ = editor.ModifyAsync(x => { x.Embed = Utils.CreateApplicationEditorEmbed(application, Context.User).Build(); x.Components = Utils.CreateApplicationEditorComponents(application, Context.User).Build(); }); //We don't care if it fails.
        }

        [ModalInteraction($"{Constants.EDIT_APP_QUESTION_MODAL}:*,*,*")]
        public async Task EditQuestion(string originalMessage, string applicationId, string selectedQuestion, EditApplicationQuestionModal modal)
        {
            var editorId = ulong.Parse(originalMessage);
            var appId = Guid.Parse(applicationId);
            var selectedQ = Guid.Parse(selectedQuestion);
            var editor = (IUserMessage)(Context.Channel.GetCachedMessage(editorId) ?? await Context.Channel.GetMessageAsync(editorId));

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
            var serverApplicationSettings = await Database.ServerApplicationSettings.FirstAsync(x => x.ServerId == server.Id);
            var application = await Database.ServerApplications.Where(x => x.ApplicationSettingsId == serverApplicationSettings.Id).FirstOrDefaultAsync(x => x.Id == appId);

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

            var affected = await Database.ServerApplicationQuestions.Where(x => x.ApplicationId == application.Id && x.Id == selectedQ).ExecuteUpdateAsync(x => x
                .SetProperty(y => y.Question, modal.Question)
                .SetProperty(y => y.Required, required)
            );

            if (affected <= 0)
            {
                await Context.Interaction.RespondOrFollowupAsync($"The question with the id {selectedQ} does not exist!", ephemeral: true);
                return;
            }

            await Context.Interaction.DeferSafelyAsync();
            _ = ModifyOriginalResponseAsync(x => { x.Content = $"Question with id {selectedQ} was successfully edited!"; x.Components = null; });
            _ = editor.ModifyAsync(x => { x.Embed = Utils.CreateApplicationEditorEmbed(application, Context.User).Build(); x.Components = Utils.CreateApplicationEditorComponents(application, Context.User).Build(); }); //We don't care if it fails.
        }
    }

    public class AddApplicationQuestionModal : IModal
    {
        public string Title => "Add Question";

        [InputLabel("Question")]
        [ModalTextInput("question", Discord.TextInputStyle.Paragraph, "Question...", maxLength: 100)]
        public string Question { get; set; } = string.Empty;

        [InputLabel("Required?")]
        [ModalTextInput("required", Discord.TextInputStyle.Short, "true", maxLength: 5, initValue: "True")]
        public string Required { get; set; } = true.ToString();
    }

    public class EditApplicationQuestionModal : IModal
    {
        public string Title => "Edit Question";

        [InputLabel("Question")]
        [ModalTextInput("question", Discord.TextInputStyle.Paragraph, "Question...", maxLength: Constants.TitleLimit)]
        public string Question { get; set; } = string.Empty;

        [InputLabel("Required?")]
        [ModalTextInput("required", Discord.TextInputStyle.Short, "true", maxLength: 5, initValue: "True")]
        public string Required { get; set; } = true.ToString();

        [InputLabel("Move")]
        [ModalTextInput("move", Discord.TextInputStyle.Short, "0", maxLength: 2, initValue: "0")]
        public string Move { get; set; } = "0";
    }
}
