using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Enclave_Bot
{
    public class EventHandler
    {
        private DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public EventHandler(IServiceProvider Services)
        {
            _services = Services;
            _client = Services.GetRequiredService<DiscordSocketClient>();
            _commands = Services.GetRequiredService<CommandService>();
        }

        public Task InitAsync()
        {
            //Create Event Listeners Here
            _client.ButtonExecuted += Button_Executed;
            _client.MessageReceived += Message_Event;
            _client.Ready += Client_Ready;
            return Task.CompletedTask;
        }

        private async Task Button_Executed(SocketMessageComponent arg)
        {
            if (arg.Data.CustomId.Contains("Accept:"))
            {
                ulong Id = ulong.Parse(arg.Data.CustomId.Replace("Accept:", ""));
                SocketGuild guild = _client.GetGuild(749358542145716275);
                SocketGuildUser UserPressed = arg.User as SocketGuildUser;
                SocketGuildUser User = guild.GetUser(Id);
                if (!UserPressed.GuildPermissions.Administrator)
                {
                    await arg.User.SendMessageAsync("Error: You do not have the correct permissions.");
                    await arg.RespondAsync(options: RequestOptions.Default);
                    return;

                }

                if (User == null)
                {
                    await arg.Message.DeleteAsync();
                    await arg.Channel.SendMessageAsync($"Error: Could not find user");
                    await arg.RespondAsync(options: RequestOptions.Default);
                    return;
                }

                await User.AddRoleAsync(guild.GetRole(757614906051919974));
                await User.RemoveRoleAsync(guild.GetRole(757613578638590013));
                await arg.Message.DeleteAsync();
                await arg.Channel.SendMessageAsync($"Accepted User: {User.Mention}");
                await User.SendMessageAsync("Congrats, you were approved, make sure to go to <#789161172528005140>🔮🏹. \n\nYou’re on your own from here, and no, we don’t have any corn 🌽");
                await arg.RespondAsync(options: RequestOptions.Default);
            }

            if (arg.Data.CustomId.Contains("Deny:"))
            {
                ulong Id = ulong.Parse(arg.Data.CustomId.Replace("Deny:", ""));
                SocketGuild guild = _client.GetGuild(749358542145716275);
                SocketGuildUser UserPressed = arg.User as SocketGuildUser;
                SocketGuildUser User = guild.GetUser(Id);
                if (!UserPressed.GuildPermissions.Administrator)
                {
                    await arg.User.SendMessageAsync("Error: You do not have the correct permissions.");
                    await arg.RespondAsync(options: RequestOptions.Default);
                    return;

                }

                if (User == null)
                {
                    await arg.Message.DeleteAsync();
                    await arg.Channel.SendMessageAsync($"Error: Could not find user");
                    await arg.RespondAsync(options: RequestOptions.Default);
                    return;
                }
                var builder = new ComponentBuilder();
                builder.WithButton("Deny Application", $"Continue:{Id}", ButtonStyle.Danger);
                IEmote[] emotes = {
                    new Emoji("1️⃣"),
                    new Emoji("2️⃣"),
                    new Emoji("3️⃣"),
                    new Emoji("4️⃣"),
                    new Emoji("5️⃣"),
                    new Emoji("6️⃣"),
                    new Emoji("7️⃣"),
                    new Emoji("8️⃣"),
                    new Emoji("9️⃣"),
                    new Emoji("🔟")};
                await arg.Message.ModifyAsync(x => x.Components = builder.Build());
                await arg.Message.AddReactionsAsync(emotes);
                await arg.RespondAsync(options: RequestOptions.Default);
            }

            if (arg.Data.CustomId.Contains("Continue:"))
            {
                ulong Id = ulong.Parse(arg.Data.CustomId.Replace("Continue:", ""));
                SocketGuild guild = _client.GetGuild(749358542145716275);
                SocketGuildUser UserPressed = arg.User as SocketGuildUser;
                SocketGuildUser User = guild.GetUser(Id);
                if (!UserPressed.GuildPermissions.Administrator)
                {
                    await arg.User.SendMessageAsync("Error: You do not have the correct permissions.");
                    await arg.RespondAsync(options: RequestOptions.Default);
                    return;

                }

                if (User == null)
                {
                    await arg.Message.DeleteAsync();
                    await arg.Channel.SendMessageAsync($"Error: Could not find user");
                    await arg.RespondAsync(options: RequestOptions.Default);
                    return;
                }

                var wrong1 = await arg.Message.GetReactionUsersAsync(new Emoji("1️⃣"), 1).FlattenAsync();
                var wrong2 = await arg.Message.GetReactionUsersAsync(new Emoji("2️⃣"), 1).FlattenAsync();
                var wrong3 = await arg.Message.GetReactionUsersAsync(new Emoji("3️⃣"), 1).FlattenAsync();
                var wrong4 = await arg.Message.GetReactionUsersAsync(new Emoji("4️⃣"), 1).FlattenAsync();
                var wrong5 = await arg.Message.GetReactionUsersAsync(new Emoji("5️⃣"), 1).FlattenAsync();
                var wrong6 = await arg.Message.GetReactionUsersAsync(new Emoji("6️⃣"), 1).FlattenAsync();
                var wrong7 = await arg.Message.GetReactionUsersAsync(new Emoji("7️⃣"), 1).FlattenAsync();
                var wrong8 = await arg.Message.GetReactionUsersAsync(new Emoji("8️⃣"), 1).FlattenAsync();
                var wrong9 = await arg.Message.GetReactionUsersAsync(new Emoji("9️⃣"), 1).FlattenAsync();
                var wrong10 = await arg.Message.GetReactionUsersAsync(new Emoji("🔟"), 1).FlattenAsync();

                string[] wrongs = { 
                    wrong1.First().ToString(), 
                    wrong2.First().ToString(),
                    wrong3.First().ToString(),
                    wrong4.First().ToString(),
                    wrong5.First().ToString(),
                    wrong6.First().ToString(),
                    wrong7.First().ToString(),
                    wrong8.First().ToString(),
                    wrong9.First().ToString(),
                    wrong10.First().ToString()
                };

                var embed = new EmbedBuilder();
                embed.Title = "Application Rejected";
                embed.Description = "The following questions to why your application was rejected are\n";
                for(int i = 0; i < 10; i++)
                {
                    if(wrongs[i] != _client.CurrentUser.ToString())
                    {
                        embed.Description += $"Question: {i + 1}\n";
                    }
                }
                await User.SendMessageAsync(embed: embed.Build());
                await arg.Message.DeleteAsync();
                await arg.Channel.SendMessageAsync($"Denied User: {User.Mention}");
                await arg.RespondAsync(options: RequestOptions.Default);
            }
        }

        private async Task Client_Ready()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{DateTime.Now} => [READY EVENT]: {_client.CurrentUser.Username} is ready");
            Console.ForegroundColor = ConsoleColor.White;
            await _client.SetGameAsync($"{Config.BotConfiguration.Prefix}help");
            await _client.SetStatusAsync(UserStatus.Online);
        }

        private async Task Message_Event(SocketMessage messageParameter)
        {
            try
            {
                SocketUserMessage message = messageParameter as SocketUserMessage;
                SocketCommandContext context = new SocketCommandContext(_client, message);

                if (context.Message == null || context.Message.Content == "")
                {
                    return;
                }

                if (context.User.IsBot)
                {
                    return;
                }

                int argPos = 0;

                if (!(message.HasStringPrefix(Config.BotConfiguration.Prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                {
                    return;
                }

                IResult result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Log.Debug($"{DateTime.Now} at Command: {_commands.Search(context, argPos).Commands[0].Command.Name} in {_commands.Search(context, argPos).Commands[0].Command.Module.Name} {result.ErrorReason}");
                    Console.ForegroundColor = ConsoleColor.White;
                    EmbedBuilder Embed = new EmbedBuilder();

                    if (result.ErrorReason == "The input text has too few paramaters")
                    {
                        Embed.WithTitle("Error");
                        Embed.WithDescription("This command requires a missing paramater. Please check help command to see what it needs");
                    }
                    else
                    {
                        Embed.WithTitle("Error");
                        Embed.WithDescription(result.ErrorReason);
                    }
                    await context.Channel.SendMessageAsync("", embed: Embed.Build());
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("[0} - {1}", e.InnerException?.Message ?? e.Message, e.StackTrace));
            }
        }
    }
}
