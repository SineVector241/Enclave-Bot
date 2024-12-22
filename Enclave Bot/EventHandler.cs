using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord.Interactions;
using Discord;
using Enclave_Bot.Database;
using Enclave_Bot.Core;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot
{
    public class EventHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _serviceProvider;
        private readonly DatabaseContext _database;

        public EventHandler(IServiceProvider services)
        {
            _serviceProvider = services;
            _client = services.GetRequiredService<DiscordSocketClient>();
            _interactions = services.GetRequiredService<InteractionService>();
            _database = services.GetRequiredService<DatabaseContext>();

            _client.Ready += ClientReady;
            _client.InteractionCreated += InteractionCreated;
            _client.ButtonExecuted += ButtonExecuted;
            _client.SelectMenuExecuted += SelectMenuExecuted;
            _client.SlashCommandExecuted += SlashCommandExecuted;
            _client.ModalSubmitted += ModalSubmitted;
            _client.AutocompleteExecuted += AutocompleteExecuted;
            _client.MessageCommandExecuted += MessageCommandExecuted;
            _client.UserCommandExecuted += UserCommandExecuted;
            _interactions.InteractionExecuted += InteractionExecuted;

            _client.MessageReceived += MessageReceived;
        }

        private Task MessageReceived(SocketMessage arg)
        {
            Task.Run(async () =>
            {
                if (Random.Shared.Next(10) != 1) return;

                if (arg.Content.Contains("sine", StringComparison.CurrentCultureIgnoreCase))
                    await arg.Channel.SendMessageAsync("He's an idiot", messageReference: new MessageReference(arg.Id));
                else if (arg.Content.Contains("skibidi", StringComparison.CurrentCultureIgnoreCase))
                    await arg.Channel.SendMessageAsync("HEY! THAT'S ILLEGAL!", messageReference: new MessageReference(arg.Id));
                else if (arg.Content.Contains("rizz", StringComparison.CurrentCultureIgnoreCase))
                    await arg.Channel.SendMessageAsync("You've got no rizz saying that.", messageReference: new MessageReference(arg.Id));
                else if (arg.Content.Contains("villager", StringComparison.CurrentCultureIgnoreCase))
                    await arg.Channel.SendMessageAsync("Those big nosed merchants sell scam deals. They're also a pain to trap.",
                        messageReference: new MessageReference(arg.Id));
                else if (arg.Content.Contains("legion on top", StringComparison.CurrentCultureIgnoreCase))
                    await arg.Channel.SendMessageAsync("No", messageReference: new MessageReference(arg.Id));
                else if (arg.Content.Contains("holiday", StringComparison.CurrentCultureIgnoreCase))
                    await arg.Channel.SendMessageAsync("Holidays? I have no holidays, WORK HARDER PEASANT.", messageReference: new MessageReference(arg.Id));
                else if (arg.Content.Contains("lost", StringComparison.CurrentCultureIgnoreCase) ||
                         arg.Content.Contains("lose", StringComparison.CurrentCultureIgnoreCase))
                    await arg.Channel.SendMessageAsync("That sucks. Too bad I win everytime.", messageReference: new MessageReference(arg.Id));
                else if (arg.Content.Contains("brb", StringComparison.CurrentCultureIgnoreCase))
                    await arg.Channel.SendMessageAsync("No you wont.", messageReference: new MessageReference(arg.Id));
                else if (arg.Content.Contains("yeet", StringComparison.CurrentCultureIgnoreCase))
                    await arg.Channel.SendMessageAsync("I'll yeet you into space.", messageReference: new MessageReference(arg.Id));
            });

            return Task.CompletedTask;
        }

        private static async Task InteractionExecuted(ICommandInfo cmdInfo, IInteractionContext ctx, IResult result)
        {
            if (!result.IsSuccess && result.Error != InteractionCommandError.UnknownCommand)
            {
                var errorEmbed = Utils.CreateError(result.ErrorReason, ctx.User);

                if (ctx.Interaction.HasResponded)
                    await ctx.Interaction.FollowupAsync(embed: errorEmbed, ephemeral: true);
                else
                    await ctx.Interaction.RespondAsync(embed: errorEmbed, ephemeral: true);
            }
        }

        //Important for the bots framework
        private async Task InteractionCreated(SocketInteraction interaction)
        {
            await _database.CreateUserIfNotExistsAsync(interaction.User);
            var user = await _database.Users.FirstAsync(u => u.Id == interaction.User.Id);
            user.LastActive = DateTime.UtcNow;
            user.Name = interaction.User.Username;

            if (interaction.GuildId != null)
            {
                var guild = _client.GetGuild(interaction.GuildId.Value);
                await _database.CreateServerIfNotExistsAsync(guild);
            }

            await _database.SaveChangesAsync();
        }

        private async Task ButtonExecuted(SocketMessageComponent interaction)
        {
            var ctx = new SocketInteractionContext<SocketMessageComponent>(_client, interaction);
            await _interactions.ExecuteCommandAsync(ctx, _serviceProvider);
        }

        private async Task SelectMenuExecuted(SocketMessageComponent interaction)
        {
            var ctx = new SocketInteractionContext<SocketMessageComponent>(_client, interaction);
            await _interactions.ExecuteCommandAsync(ctx, _serviceProvider);
        }

        private async Task SlashCommandExecuted(SocketSlashCommand interaction)
        {
            var ctx = new SocketInteractionContext<SocketSlashCommand>(_client, interaction);
            await _interactions.ExecuteCommandAsync(ctx, _serviceProvider);
        }

        private async Task ModalSubmitted(SocketModal interaction)
        {
            var ctx = new SocketInteractionContext<SocketModal>(_client, interaction);
            await _interactions.ExecuteCommandAsync(ctx, _serviceProvider);
        }

        private async Task AutocompleteExecuted(SocketAutocompleteInteraction interaction)
        {
            var ctx = new SocketInteractionContext<SocketAutocompleteInteraction>(_client, interaction);
            await _interactions.ExecuteCommandAsync(ctx, _serviceProvider);
        }

        private async Task MessageCommandExecuted(SocketMessageCommand interaction)
        {
            var ctx = new SocketInteractionContext<SocketMessageCommand>(_client, interaction);
            await _interactions.ExecuteCommandAsync(ctx, _serviceProvider);
        }

        private async Task UserCommandExecuted(SocketUserCommand interaction)
        {
            var ctx = new SocketInteractionContext<SocketUserCommand>(_client, interaction);
            await _interactions.ExecuteCommandAsync(ctx, _serviceProvider);
        }

        private async Task ClientReady()
        {
            Console.WriteLine($"[{DateTime.Now}]: [READY] => {_client.CurrentUser.Username} is ready!");
            await _client.SetGameAsync("/help");
            await _client.SetStatusAsync(UserStatus.Online);
            await _interactions.RegisterCommandsGloballyAsync();
        }
    }
}