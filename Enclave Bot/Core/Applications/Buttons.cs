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

        [ComponentInteraction($"{Constants.ADD_APP_QUESTION}:*,*")]
        public async Task AddQuestion(string author, string applicationId)
        {
            var owner = ulong.Parse(author);
            var appId = Guid.Parse(applicationId);

            if (Context.User.Id != owner)
            {
                await Context.Interaction.RespondOrFollowupAsync("You are not the owner of this editor!");
                return;
            }

            await RespondWithModalAsync<AddApplicationQuestionModal>($"{Constants.ADD_APP_QUESTION_MODAL}:{owner},{appId}");
        }
    }
}