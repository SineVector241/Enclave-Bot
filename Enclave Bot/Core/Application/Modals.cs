using Discord.Interactions;
using Discord;
using Enclave_Bot.Database;
using Discord.WebSocket;
using Enclave_Bot.Extensions;

namespace Enclave_Bot.Core.Application
{
    public class Modals(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketModal>>
    {
        private readonly DatabaseContext Database = database;

        [ModalInteraction("AAQM:*")]
        public async Task AddAppQuestion(string applicationId, AddAppQuestionModal modal)
        {
            var appId = Guid.Parse(applicationId);
            var indexSuccess = int.TryParse(modal.Index, out int index);

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);
            var application = Database.ServerApplications.Where(x => x.ServerId == server.Id).FirstOrDefault(x => x.Id == appId);

            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The application `{applicationId}` no longer exists!", Context.User), ephemeral: true);
                return;
            }
            if (application.Questions.Count > Bot.QuestionsLimit)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The application `{application.Name}` has reached the maximum question limit, cannot add question.", Context.User), ephemeral: true);
                return;
            }

            if (!indexSuccess || index >= Bot.QuestionsLimit || index < 0 || index > application.Questions.Count - 1)
            {
                application.Questions.Add(modal.Question);
            }
            else
            {
                application.Questions.Insert(index, modal.Question);
            }
            await Context.Interaction.DeferSafelyAsync(ephemeral: true);
            await Database.SaveChangesAsync();

            var msg = await Context.Interaction.GetOriginalResponseAsync();
            if (msg == null)
            {
                await Context.Interaction.RespondOrFollowupAsync("Question successfully added.", ephemeral: true);
                return;
            }

            var embed = Utils.CreateApplicationEditorEmbed(application, Context.User);
            await msg.ModifyAsync(x => { x.Embed = embed.Build(); });
            await Context.Interaction.RespondOrFollowupAsync("Question successfully added.", ephemeral: true);
        }

        [ModalInteraction("EAQM:*,*,*")]
        public async Task EditAppQuestion(string applicationId, string selectedQuestion, string messageId, EditAppQuestionModal modal)
        {
            var appId = Guid.Parse(applicationId);
            var msgId = ulong.Parse(messageId);
            var sltdQ = int.Parse(selectedQuestion);

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);
            var application = Database.ServerApplications.Where(x => x.ServerId == server.Id).FirstOrDefault(x => x.Id == appId);

            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The application `{applicationId}` no longer exists!", Context.User), ephemeral: true);
                return;
            }

            application.Questions[sltdQ] = modal.Question;

            await Context.Interaction.DeferSafelyAsync(ephemeral: true);
            await Database.SaveChangesAsync();

            var msg = (IUserMessage)await Context.Channel.GetMessageAsync(msgId);
            if (msg == null)
            {
                await Context.Interaction.RespondOrFollowupAsync("Question successfully edited.", ephemeral: true);
                return;
            }

            var embed = Utils.CreateApplicationEditorEmbed(application, Context.User);
            await msg.ModifyAsync(x => { x.Embed = embed.Build(); });
            await Context.Interaction.RespondOrFollowupAsync("Question successfully edited.", ephemeral: true);
        }
    }

    public class AddAppQuestionModal : IModal
    {
        public string Title => "test";

        [InputLabel("Question")]
        [ModalTextInput("question", TextInputStyle.Paragraph, "Question", 1, 100)]
        public string Question { get; set; } = string.Empty;

        [InputLabel("Index")]
        [ModalTextInput("index", TextInputStyle.Short, "0", 1, 2, "24")]
        public string Index { get; set; } = "24";
    }

    public class EditAppQuestionModal : IModal
    {
        public string Title => "test";

        [InputLabel("Question")]
        [ModalTextInput("question", TextInputStyle.Paragraph, "Question", 1, 100)]
        public string Question { get; set; } = string.Empty;
    }
}