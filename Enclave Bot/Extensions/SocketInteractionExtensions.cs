using Discord;
using Discord.WebSocket;

namespace Enclave_Bot.Extensions
{
    public static class SocketInteractionExtensions
    {
        public static async Task RespondOrFollowupAsync<TInteraction>(this TInteraction interaction, string? text = null, Embed[]? embeds = null, bool isTTS = false, bool ephemeral = false,
             AllowedMentions? allowedMentions = null, MessageComponent? components = null, Embed? embed = null, RequestOptions? options = null, PollProperties? poll = null) where TInteraction : SocketInteraction
        {
            if(interaction.HasResponded)
            {
                await interaction.FollowupAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
                return;
            }
            await interaction.RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, components, embed, options);
        }

        public static async Task DeferSafelyAsync<TInteraction>(this TInteraction interaction, bool ephemeral = false, RequestOptions? options = null) where TInteraction : SocketInteraction
        {
            if (interaction.HasResponded)
                return;
            await interaction.DeferAsync(ephemeral, options);
        }
    }
}
