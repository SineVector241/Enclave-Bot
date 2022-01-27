using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;

namespace Enclave_Bot.Core.Interactions
{
    public class ReactionRoleInteractions : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        public InteractiveService Interactive { get; set; }

        [ComponentInteraction("RRDashAdd:*")]
        public async Task ReactionRoleDashAdd(string userid)
        {
            try
            {
                EmbedBuilder embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.First());
                if (userid != Context.User.Id.ToString())
                {
                    await RespondAsync("This is not your dashboard", ephemeral: true);
                    return;
                }
                await DeferAsync();
                var msgrole = await Context.Channel.SendMessageAsync("Please mention or send the role ID you want to set");
                var roleID = await Interactive.NextMessageAsync(x => x.Channel == Context.Channel && x.Author.Id == Context.User.Id, timeout: TimeSpan.FromMinutes(2));
                if (!roleID.IsSuccess)
                {
                    await Context.Channel.SendMessageAsync("Response Timed Out. Canceling Action");
                    return;
                }

                var roleid = System.Text.RegularExpressions.Regex.Replace(roleID.Value.Content, "[^0-9]", "");
                var role = Context.Guild.GetRole((ulong)Convert.ToInt64(roleid));
                embed.AddField(role.Name, role.Id);
                await Context.Interaction.Message.ModifyAsync(x => x.Embed = (embed.Build()));
                await msgrole.DeleteAsync();
                await roleID.Value.DeleteAsync();
            }
            catch(Exception ex)
            {
                await Context.Channel.SendMessageAsync("Oops an error occured");
            }
        }

        [ComponentInteraction("RRDashRemove:*")]
        public async Task ReactionRoleDashRemove(string userid)
        {
            try
            {
                EmbedBuilder embed = EmbedBuilderExtensions.ToEmbedBuilder(Context.Interaction.Message.Embeds.First());
                if (userid != Context.User.Id.ToString())
                {
                    await RespondAsync("This is not your dashboard", ephemeral: true);
                    return;
                }
                await DeferAsync();
                var msgrole = await Context.Channel.SendMessageAsync("Please mention or send the ID of the role you want to remove");
                var roleID = await Interactive.NextMessageAsync(x => x.Channel == Context.Channel && x.Author.Id == Context.User.Id, timeout: TimeSpan.FromMinutes(2));
                if (!roleID.IsSuccess)
                {
                    await Context.Channel.SendMessageAsync("Response Timed Out. Canceling Action");
                    return;
                }

                var roleid = System.Text.RegularExpressions.Regex.Replace(roleID.Value.Content, "[^0-9]", "");
                var role = Context.Guild.GetRole((ulong)Convert.ToInt64(roleid));
                foreach (var field in embed.Fields)
                {
                    if (field.Value.ToString().Contains($"roleID:{role.Id}"))
                    {
                        embed.Fields.Remove(field);
                        break;
                    }
                }
                await Context.Interaction.Message.ModifyAsync(x => x.Embed = embed.Build());
                await msgrole.DeleteAsync();
                await roleID.Value.DeleteAsync();
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync("Oops an error occured");
            }
        }

        [ComponentInteraction("SendRR:*,*")]
        public async Task SendReactionRoleSetup(string channelID, string userid)
        {
            try
            {
                var embed = Context.Interaction.Message.Embeds.First();
                var builder = new ComponentBuilder();
                if (userid != Context.User.Id.ToString())
                {
                    await RespondAsync("This is not your dashboard", ephemeral: true);
                    return;
                }
                await DeferAsync();
                foreach (var field in embed.Fields)
                {
                    builder.WithButton(customId: $"RR:{field.Value}", label: field.Name);
                }
                var channel = Context.Guild.GetChannel((ulong)Convert.ToInt64(channelID)) as SocketTextChannel;
                await channel.SendMessageAsync(embed: new EmbedBuilder().WithTitle("Reaction Roles").Build(), components: builder.Build());
                await Context.Interaction.Message.DeleteAsync();
            }
            catch(Exception ex)
            {
                await Context.Channel.SendMessageAsync("Oops an error occured");
            }
        }

        [ComponentInteraction("RR:*")]
        public async Task ReactionRole(string roleID)
        {
            SocketGuildUser? user = Context.User as SocketGuildUser;
            var role = Context.Guild.GetRole((ulong)Convert.ToInt64(roleID));
            if (user.Roles.Contains(role))
            {
                await user.RemoveRoleAsync(role);
                await RespondAsync("Successfully Removed Role", ephemeral: true);
            }
            else
            {
                await user.AddRoleAsync(role);
                await RespondAsync("Successfully Added Role", ephemeral: true);
            }
        }
    }
}