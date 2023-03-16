using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using System.Text.RegularExpressions;

namespace Enclave_Bot.Core.ReactionRole
{
    public class Builder : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        public InteractiveService Interactive { get; set; }

        [ComponentInteraction("RRBA:*")]
        public async Task AddRole(string UserId)
        {
            try
            {
                var embed = Context.Interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
                if (UserId != Context.User.Id.ToString())
                {
                    await RespondAsync("You cannot use this!", ephemeral: true);
                    return;
                }

                await RespondAsync("Please mention or send the ID of the required role you want to set", ephemeral: true);
                var roleResult = await GetRoleResponseAsync();
                if (roleResult == null) return;

                await FollowupAsync("Please send the title of the button you want to set for this role", ephemeral: true);
                var buttonTitle = await Interactive.NextMessageAsync(x => x.Author.Id == Context.User.Id && x.Channel.Id == Context.Channel.Id, timeout: TimeSpan.FromMinutes(5));
                if (!buttonTitle.IsSuccess)
                {
                    await FollowupAsync("Error: Timed Out");
                    return;
                }
                await buttonTitle.Value.DeleteAsync();

                embed.AddField(buttonTitle.Value.Content, roleResult.Id.ToString());
                await Context.Interaction.Message.ModifyAsync(x => x.Embed = embed.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [ComponentInteraction("RRBD:*")]
        public async Task RemoveRole(string UserId)
        {
            var embed = Context.Interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
            if (UserId != Context.User.Id.ToString())
            {
                await RespondAsync("You cannot use this!", ephemeral: true);
                return;
            }

            await RespondAsync("Please mention or send the ID of the role you want to remove", ephemeral: true);
            var roleResult = await GetRoleResponseAsync();
            if (roleResult == null) return;

            var role = embed.Fields.FirstOrDefault(x => x.Value.ToString() == roleResult.Id.ToString());
            if (role == null)
            {
                await FollowupAsync("Error. Cannot find role in builder", ephemeral: true);
                return;
            }

            embed.Fields.Remove(role);

            await Context.Interaction.Message.ModifyAsync(x => x.Embed = embed.Build());
        }

        [ComponentInteraction("RRBST:*")]
        public async Task SetTitle(string UserId)
        {
            var embed = Context.Interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
            if (UserId != Context.User.Id.ToString())
            {
                await RespondAsync("You cannot use this!", ephemeral: true);
                return;
            }

            await RespondAsync("Please send the title you want to set for the embed", ephemeral: true);
            var result = await Interactive.NextMessageAsync(x => x.Author.Id == Context.User.Id && x.Channel.Id == Context.Channel.Id, timeout: TimeSpan.FromMinutes(5));
            if (!result.IsSuccess)
            {
                await FollowupAsync("Error: Timed Out");
                return;
            }
            await result.Value.DeleteAsync();

            embed.WithTitle(result.Value.Content);
            await Context.Interaction.Message.ModifyAsync(x => x.Embed = embed.Build());
        }

        [ComponentInteraction("RRBSD:*")]
        public async Task SetDescription(string UserId)
        {
            var embed = Context.Interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
            if (UserId != Context.User.Id.ToString())
            {
                await RespondAsync("You cannot use this!", ephemeral: true);
                return;
            }

            await RespondAsync("Please send the description you want to set for the embed", ephemeral: true);
            var result = await Interactive.NextMessageAsync(x => x.Author.Id == Context.User.Id && x.Channel.Id == Context.Channel.Id, timeout: TimeSpan.FromMinutes(5));
            if (!result.IsSuccess)
            {
                await FollowupAsync("Error: Timed Out");
                return;
            }
            await result.Value.DeleteAsync();

            embed.WithDescription(result.Value.Content);
            await Context.Interaction.Message.ModifyAsync(x => x.Embed = embed.Build());
        }

        [ComponentInteraction("RRBT:*")]
        public async Task SetType(string UserId, string[] selections)
        {
            var selection = selections.FirstOrDefault();
            var embed = Context.Interaction.Message.Embeds.FirstOrDefault().ToEmbedBuilder();
            if (UserId != Context.User.Id.ToString())
            {
                await RespondAsync("You cannot use this!", ephemeral: true);
                return;
            }

            var valueType = "Normal";
            var requiredRole = "None";
            Regex requireRoleRegex = new Regex(@"(?<=RequiredRole: ).*(?=)", RegexOptions.Multiline);

            switch (selection)
            {
                case "1":
                    await DeferAsync();
                    embed.WithFooter(requireRoleRegex.Replace(embed.Footer.Text, requiredRole));
                    break;

                case "2":
                    valueType = "1 Role Only";
                    await DeferAsync();
                    embed.WithFooter(requireRoleRegex.Replace(embed.Footer.Text, requiredRole));
                    break;

                case "3":
                    valueType = "Add & Remove Role";
                    await RespondAsync("Please mention or send the ID of the removal role you want to set", ephemeral: true);
                    var roleResult = await GetRoleResponseAsync();
                    if (roleResult == null) return;
                    requiredRole = roleResult.Id.ToString();
                    embed.WithFooter(requireRoleRegex.Replace(embed.Footer.Text, requiredRole));
                    break;

                case "4":
                    valueType = "Add & Remove Required Role";
                    await RespondAsync("Please mention or send the ID of the required role you want to set", ephemeral: true);
                    var roleResult1 = await GetRoleResponseAsync();
                    if (roleResult1 == null) return;
                    requiredRole = roleResult1.Id.ToString();
                    embed.WithFooter(requireRoleRegex.Replace(embed.Footer.Text, requiredRole));
                    break;

                case "5":
                    valueType = "Normal & Requires Role";
                    await RespondAsync("Please mention or send the ID of the required role you want to set", ephemeral: true);
                    var roleResult2 = await GetRoleResponseAsync();
                    if (roleResult2 == null) return;
                    requiredRole = roleResult2.Id.ToString();
                    embed.WithFooter(requireRoleRegex.Replace(embed.Footer.Text, requiredRole));
                    break;
            }

            Regex regex = new Regex(@"(?<=Type: ).*(?=,)", RegexOptions.Multiline);
            embed.WithFooter(regex.Replace(embed.Footer.Text, valueType));

            await Context.Interaction.Message.ModifyAsync(x => x.Embed = embed.Build());
        }

        [ComponentInteraction("RRBP:*,*")]
        public async Task Publish(string UserId, string ChannelId)
        {
            var embed = Context.Interaction.Message.Embeds.FirstOrDefault();
            if (UserId != Context.User.Id.ToString())
            {
                await RespondAsync("You cannot use this!", ephemeral: true);
                return;
            }

            if(embed.Fields.Length == 0)
            {
                await RespondAsync("Error. No Reaction Roles Set!!", ephemeral: true);
                return;
            }

            var channel = Context.Guild.GetTextChannel(ulong.Parse(ChannelId));
            var roles = embed.Fields;
            var builder = new ComponentBuilder();

            switch(Regex.Match(embed.Footer.Value.Text, @"(?<=Type: ).*(?=,)", RegexOptions.Multiline).Value)
            {
                case "Normal":
                    for(int i = 0; i < roles.Length; i++)
                    {
                        var role = roles[i];
                        builder.WithButton(role.Name, $"RR0:{role.Value}");
                    }
                    break;
                case "1 Role Only":
                    for (int i = 0; i < roles.Length; i++)
                    {
                        var role = roles[i];
                        builder.WithButton(role.Name, $"RR1:{role.Value}");
                    }
                    break;
                case "Add & Remove Role":
                    for (int i = 0; i < roles.Length; i++)
                    {
                        var role = roles[i];
                        builder.WithButton(role.Name, $"RR2:{role.Value},{Regex.Match(embed.Footer.Value.Text, @"(?<=RequiredRole: ).*(?=)").Value}");
                    }
                    break;
                case "Add & Remove Required Role":
                    for (int i = 0; i < roles.Length; i++)
                    {
                        var role = roles[i];
                        builder.WithButton(role.Name, $"RR3:{role.Value},{Regex.Match(embed.Footer.Value.Text, @"(?<=RequiredRole: ).*(?=)").Value}");
                    }
                    break;
                case "Normal & Requires Role":
                    for (int i = 0; i < roles.Length; i++)
                    {
                        var role = roles[i];
                        builder.WithButton(role.Name, $"RR4:{role.Value},{Regex.Match(embed.Footer.Value.Text, @"(?<=RequiredRole: ).*(?=)").Value}");
                    }
                    break;
            }

            var RREmbed = new EmbedBuilder()
                .WithTitle(embed.Title)
                .WithDescription(embed.Description)
                .WithColor(Utils.RandomColor())
                .Build();

            await channel.SendMessageAsync(embed: RREmbed, components: builder.Build());

            await Context.Interaction.Message.DeleteAsync();
        }

        private async Task<SocketRole?> GetRoleResponseAsync()
        {
            var roleResult = await Interactive.NextMessageAsync(x => x.Author.Id == Context.User.Id && x.Channel.Id == Context.Channel.Id, timeout: TimeSpan.FromMinutes(5));
            if (!roleResult.IsSuccess)
            {
                await FollowupAsync("Error: Timed Out");
                return null;
            }
            await roleResult.Value.DeleteAsync();

            ulong resultId = 0;
            ulong.TryParse(string.Concat(roleResult.Value.Content.Where(char.IsDigit)), out resultId);

            Console.WriteLine(string.Concat(roleResult.Value.Content.Where(char.IsDigit)));

            var role = Context.Guild.GetRole(resultId);

            if (role == null)
            {
                await FollowupAsync("Error. Cannot find role", ephemeral: true);
                return null;
            }

            return role;
        }
    }
}
