using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;

namespace Enclave_Bot.Core.Application
{
    public class SelectMenus(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        private readonly DatabaseContext Database = database;

        [ComponentInteraction("DAQ:*,*")]
        public async Task DeleteAppQuestion(string applicationId, string messageId, string[] selectedQuestions)
        {
            var appId = Guid.Parse(applicationId);
            var msgId = ulong.Parse(messageId);
            var sltdQs = selectedQuestions.Select(int.Parse).ToArray();

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);
            var application = Database.ServerApplications.Where(x => x.ServerId == server.Id).FirstOrDefault(x => x.Id == appId);

            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The application `{applicationId}` no longer exists!", Context.User), ephemeral: true);
                return;
            }

            for (var i = application.Questions.Count - 1; i >= 0; i--)
            {
                if (sltdQs.Contains(i))
                {
                    application.Questions.RemoveAt(i);
                }
            }

            await Context.Interaction.DeferSafelyAsync(true);
            await Database.SaveChangesAsync();

            var msg = (IUserMessage)await Context.Channel.GetMessageAsync(msgId);
            if (msg == null)
            {
                await Context.Interaction.RespondOrFollowupAsync("Question successfully added.", ephemeral: true);
                return;
            }

            var embed = Utils.CreateApplicationEditorEmbed(application, Context.User);
            await msg.ModifyAsync(x => { x.Embed = embed.Build(); });
            await Context.Interaction.DeleteOriginalResponseAsync();
            await Context.Interaction.RespondOrFollowupAsync("Questions successfully deleted.", ephemeral: true);
        }

        [ComponentInteraction("ESAQ:*,*")]
        public async Task EditSelectAppQuestion(string applicationId, string messageId, string[] selectedQuestion)
        {
            var appId = Guid.Parse(applicationId);
            var msgId = ulong.Parse(messageId);
            var sltdQ = int.Parse(selectedQuestion[0]);

            var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction, true);
            var application = Database.ServerApplications.Where(x => x.ServerId == server.Id).FirstOrDefault(x => x.Id == appId);

            if (application == null)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The application `{applicationId}` no longer exists!", Context.User), ephemeral: true);
                return;
            }
            if (application.Questions.Count < sltdQ)
            {
                await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed("The selected question no longer exists!", Context.User), ephemeral: true);
                return;
            }

            var title = $"Editing Question: {application.Name}";
            await RespondWithModalAsync<EditAppQuestionModal>($"EAQM:{applicationId},{sltdQ},{msgId}", modifyModal: x =>
            {
                x.WithTitle(title.Truncate(Bot.TitleLengthLimit));
                x.UpdateTextInput("question", y => y.WithValue(application.Questions[sltdQ]));
            });
        }
    }
}
