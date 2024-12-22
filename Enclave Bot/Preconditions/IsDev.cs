using Discord;
using Discord.Interactions;

namespace Enclave_Bot.Preconditions
{
    public class IsDev : PreconditionAttribute
    {
        private static readonly ulong[] DevIds = [
            550912080627236874, //Sine Vector
            646548729981173790 //Haydan
        ];
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            return Task.FromResult(DevIds.Contains(context.User.Id) ? PreconditionResult.FromSuccess() : PreconditionResult.FromError($"User is not a dev!"));
        }
    }
}