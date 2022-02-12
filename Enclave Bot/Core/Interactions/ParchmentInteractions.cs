using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;

namespace Enclave_Bot.Core.Interactions
{
    public class ParchmentInteractions : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        private Database.Database db = new Database.Database();

        [ComponentInteraction("OpenParchment")]
        public async Task OpenParchment()
        {
            var settings = await db.GetGuildSettingsById(Context.Guild.Id);
            foreach (SocketTextChannel chan in Context.Guild.GetCategoryChannel(settings.ParchmentCategory).Channels)
            {
                if(chan.Name == Context.User.Username)
                {
                    await RespondAsync("You already have a parchment open. Please close it to open a new one!", ephemeral: true);
                }
            }
            var channel = await Context.Guild.CreateTextChannelAsync(Context.User.Username, func: x => { x.CategoryId = settings.ParchmentCategory; });
            await channel.SendMessageAsync($"{Context.User.Mention} Here is your parchment. Please describe what you opened this ticket for. A staff member will reply to your ticket shortly");
            await RespondAsync("Created your parchment",ephemeral: true);
        }
    }
}
