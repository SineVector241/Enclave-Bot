using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Preconditions;

namespace Enclave_Bot.Core.Actions
{
    [IsStaff]
    public class Buttons(DatabaseContext database) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        [ComponentInteraction($"{Constants.SERVER_ACTION_LIST_CREATE}:*")]
        public async Task ServerActionListCreate(string sAuthorId)
        {
            var authorId = ulong.Parse(sAuthorId);
            if (authorId != Context.User.Id)
            {
                await RespondAsync("You are not the owner of this actions list!", ephemeral: true);
                return;
            }

            await RespondWithModalAsync<CreateActionModal>($"{Constants.SERVER_ACTION_MODAL_CREATE}");
        }
    }
}