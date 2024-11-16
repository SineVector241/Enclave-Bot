using Discord.Interactions;
using Discord.WebSocket;
using Enclave_Bot.Database;
using Enclave_Bot.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Core.Actions;

public class Buttons(DatabaseContext database, Utils utils) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private readonly DatabaseContext Database = database;
    private readonly Utils Utils = utils;

    [ComponentInteraction("ASAB:*,*")]
    public async Task AddBehavior(string actionId, string userId)
    {
        var actId = Guid.Parse(actionId);
        var ownerId = ulong.Parse(userId);
        
        var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
        var action = await Database.ServerActions.Where(x => x.ServerId == server.Id).FirstOrDefaultAsync(x => x.Id == actId);

        if (ownerId != Context.User.Id)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed("You are not the owner of this action editor!", Context.User), ephemeral: true);
            return;
        }
        if (action == null)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The action `{actId}` no longer exists!", Context.User), ephemeral: true);
            return;
        }

        await Context.Interaction.RespondOrFollowupAsync("STUFF TO DO HERE!");
    }
    
    [ComponentInteraction("RSAB:*,*")]
    public async Task RemoveBehavior(string actionId, string userId)
    {
        var actId = Guid.Parse(actionId);
        var ownerId = ulong.Parse(userId);
        
        var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
        var action = await Database.ServerActions.Where(x => x.ServerId == server.Id).FirstOrDefaultAsync(x => x.Id == actId);

        if (ownerId != Context.User.Id)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed("You are not the owner of this action editor!", Context.User), ephemeral: true);
            return;
        }
        if (action == null)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The action `{actId}` no longer exists!", Context.User), ephemeral: true);
            return;
        }

        await Context.Interaction.RespondOrFollowupAsync("STUFF TO DO HERE!");
    }
    
    [ComponentInteraction("ESAB:*,*")]
    public async Task EditBehavior(string actionId, string userId)
    {
        var actId = Guid.Parse(actionId);
        var ownerId = ulong.Parse(userId);
        
        var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
        var action = await Database.ServerActions.Where(x => x.ServerId == server.Id).FirstOrDefaultAsync(x => x.Id == actId);

        if (ownerId != Context.User.Id)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed("You are not the owner of this action editor!", Context.User), ephemeral: true);
            return;
        }
        if (action == null)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The action `{actId}` no longer exists!", Context.User), ephemeral: true);
            return;
        }

        await Context.Interaction.RespondOrFollowupAsync("STUFF TO DO HERE!");
    }
    
    [ComponentInteraction("SSAC:*,*")]
    public async Task SwitchToConditions(string actionId, string userId)
    {
        var actId = Guid.Parse(actionId);
        var ownerId = ulong.Parse(userId);
        
        var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
        var action = await Database.ServerActions.Where(x => x.ServerId == server.Id).FirstOrDefaultAsync(x => x.Id == actId);

        if (ownerId != Context.User.Id)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed("You are not the owner of this action editor!", Context.User), ephemeral: true);
            return;
        }
        if (action == null)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The action `{actId}` no longer exists!", Context.User), ephemeral: true);
            return;
        }

        var embed = Utils.CreateServerActionConditionsEditorEmbed(action, Context.User);
        var components = Utils.CreateServerActionConditionsEditorComponents(action, Context.User);
        await Context.Interaction.DeferSafelyAsync();
        await ModifyOriginalResponseAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = components.Build();
        });
    }
    
    [ComponentInteraction("ASAC:*,*")]
    public async Task AddCondition(string actionId, string userId)
    {
        var actId = Guid.Parse(actionId);
        var ownerId = ulong.Parse(userId);
        
        var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
        var action = await Database.ServerActions.Where(x => x.ServerId == server.Id).FirstOrDefaultAsync(x => x.Id == actId);

        if (ownerId != Context.User.Id)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed("You are not the owner of this action editor!", Context.User), ephemeral: true);
            return;
        }
        if (action == null)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The action `{actId}` no longer exists!", Context.User), ephemeral: true);
            return;
        }

        await Context.Interaction.RespondOrFollowupAsync("STUFF TO DO HERE!");
    }
    
    [ComponentInteraction("RSAC:*,*")]
    public async Task RemoveCondition(string actionId, string userId)
    {
        var actId = Guid.Parse(actionId);
        var ownerId = ulong.Parse(userId);
        
        var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
        var action = await Database.ServerActions.Where(x => x.ServerId == server.Id).FirstOrDefaultAsync(x => x.Id == actId);

        if (ownerId != Context.User.Id)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed("You are not the owner of this action editor!", Context.User), ephemeral: true);
            return;
        }
        if (action == null)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The action `{actId}` no longer exists!", Context.User), ephemeral: true);
            return;
        }

        await Context.Interaction.RespondOrFollowupAsync("STUFF TO DO HERE!");
    }
    
    [ComponentInteraction("ESAC:*,*")]
    public async Task EditCondition(string actionId, string userId)
    {
        var actId = Guid.Parse(actionId);
        var ownerId = ulong.Parse(userId);
        
        var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
        var action = await Database.ServerActions.Where(x => x.ServerId == server.Id).FirstOrDefaultAsync(x => x.Id == actId);

        if (ownerId != Context.User.Id)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed("You are not the owner of this action editor!", Context.User), ephemeral: true);
            return;
        }
        if (action == null)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The action `{actId}` no longer exists!", Context.User), ephemeral: true);
            return;
        }

        await Context.Interaction.RespondOrFollowupAsync("STUFF TO DO HERE!");
    }
    
    [ComponentInteraction("SSAB:*,*")]
    public async Task SwitchToBehaviors(string actionId, string userId)
    {
        var actId = Guid.Parse(actionId);
        var ownerId = ulong.Parse(userId);
        
        var server = await Database.GetOrCreateServerById(Context.Guild.Id, Context.Interaction);
        var action = await Database.ServerActions.Where(x => x.ServerId == server.Id).FirstOrDefaultAsync(x => x.Id == actId);

        if (ownerId != Context.User.Id)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed("You are not the owner of this action editor!", Context.User), ephemeral: true);
            return;
        }
        if (action == null)
        {
            await Context.Interaction.RespondOrFollowupAsync(embed: Utils.CreateErrorEmbed($"The action `{actId}` no longer exists!", Context.User), ephemeral: true);
            return;
        }

        var embed = Utils.CreateServerActionBehaviorsEditorEmbed(action, Context.User);
        var components = Utils.CreateServerActionBehaviorsEditorComponents(action, Context.User);
        await Context.Interaction.DeferSafelyAsync();
        await ModifyOriginalResponseAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = components.Build();
        });
    }
}