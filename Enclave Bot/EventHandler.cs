using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord.Interactions;
using Discord;
using Enclave_Bot.Database;
using Enclave_Bot.Core;

namespace Enclave_Bot
{
    public class EventHandler
    {
        private readonly DiscordSocketClient Client;
        private readonly InteractionService Interactions;
        private readonly IServiceProvider ServiceProvider;
        private readonly DatabaseContext Database;

        public EventHandler(IServiceProvider services)
        {
            ServiceProvider = services;
            Client = services.GetRequiredService<DiscordSocketClient>();
            Interactions = services.GetRequiredService<InteractionService>();
            Database = services.GetRequiredService<DatabaseContext>();

            Client.Ready += ClientReady;
            Client.InteractionCreated += InteractionCreated;
            Client.ButtonExecuted += ButtonExecuted;
            Client.SelectMenuExecuted += SelectMenuExecuted;
            Client.SlashCommandExecuted += SlashCommandExecuted;
            Client.ModalSubmitted += ModalSubmitted;
            Client.AutocompleteExecuted += AutocompleteExecuted;
            Client.MessageCommandExecuted += MessageCommandExecuted;
            Client.UserCommandExecuted += UserCommandExecuted;
            Interactions.InteractionExecuted += InteractionExecuted;
        }

        private async Task InteractionExecuted(ICommandInfo cmdInfo, IInteractionContext ctx, IResult result)
        {
            if (!result.IsSuccess && result.Error != InteractionCommandError.UnknownCommand)
            {
                var utils = ServiceProvider.GetRequiredService<Utils>();
                var errorEmbed = utils.CreateErrorEmbed(result.ErrorReason, ctx.User);

                if (ctx.Interaction.HasResponded)
                    await ctx.Interaction.FollowupAsync(embed: errorEmbed);
                else
                    await ctx.Interaction.RespondAsync(embed: errorEmbed);
            }
        }

        //Important for the bots framework
        private async Task InteractionCreated(SocketInteraction interaction)
        {
            var user = await Database.GetOrCreateUserById(interaction.User.Id);
            user.LastActive = DateTime.UtcNow;
            user.Username = interaction.User.Username;
            await Database.SaveChangesAsync();
        }

        private async Task ButtonExecuted(SocketMessageComponent interaction)
        {
            var ctx = new SocketInteractionContext<SocketMessageComponent>(Client, interaction);
            await Interactions.ExecuteCommandAsync(ctx, ServiceProvider);
        }

        private async Task SelectMenuExecuted(SocketMessageComponent interaction)
        {
            var ctx = new SocketInteractionContext<SocketMessageComponent>(Client, interaction);
            await Interactions.ExecuteCommandAsync(ctx, ServiceProvider);
        }

        private async Task SlashCommandExecuted(SocketSlashCommand interaction)
        {
            var ctx = new SocketInteractionContext<SocketSlashCommand>(Client, interaction);
            await Interactions.ExecuteCommandAsync(ctx, ServiceProvider);
        }

        private async Task ModalSubmitted(SocketModal interaction)
        {
            var ctx = new SocketInteractionContext<SocketModal>(Client, interaction);
            await Interactions.ExecuteCommandAsync(ctx, ServiceProvider);
        }

        private async Task AutocompleteExecuted(SocketAutocompleteInteraction interaction)
        {
            var ctx = new SocketInteractionContext<SocketAutocompleteInteraction>(Client, interaction);
            await Interactions.ExecuteCommandAsync(ctx, ServiceProvider);
        }

        private async Task MessageCommandExecuted(SocketMessageCommand interaction)
        {
            var ctx = new SocketInteractionContext<SocketMessageCommand>(Client, interaction);
            await Interactions.ExecuteCommandAsync(ctx, ServiceProvider);
        }

        private async Task UserCommandExecuted(SocketUserCommand interaction)
        {
            var ctx = new SocketInteractionContext<SocketUserCommand>(Client, interaction);
            await Interactions.ExecuteCommandAsync(ctx, ServiceProvider);
        }

        private async Task ClientReady()
        {
            Console.WriteLine($"[{DateTime.Now}]: [READY] => {Client.CurrentUser.Username} is ready!");
            await Client.SetGameAsync("/help");
            await Client.SetStatusAsync(UserStatus.Online);
            await Interactions.RegisterCommandsGloballyAsync();
        }
    }
}
