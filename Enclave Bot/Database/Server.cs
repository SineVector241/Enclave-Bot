﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Discord;
using Enclave_Bot.Core;

namespace Enclave_Bot.Database
{
    [PrimaryKey(nameof(Id))]
    public class Server
    {
        [Required] public required ulong Id { get; init; }

        [Required] public ServerSettings Settings { get; init; } = new();
        
        [Required] public LogSettings LogSettings { get; init; } = new();
        [Required] public ApplicationSettings ApplicationSettings { get; init; } = new();

        [Required] public ServerActionsSettings ServerActionsSettings { get; init; } = new();
        
        [Timestamp]
        public byte[]? Version { get; set; }
    }
    
    #region Server Settings
    [PrimaryKey(nameof(Id))]
    public class ServerSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public Guid Id { get; set; }
        
        [Required] public List<ulong> StaffRoles { get; set; } = [];
        
        [Timestamp]
        public byte[]? Version { get; set; }
    }
    #endregion

    #region Log Settings
    [PrimaryKey(nameof(Id))]
    public class LogSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public Guid Id { get; set; } 
        
        [Required] public List<LogSetting> Settings { get; set; } = [];
        
        public ulong? DefaultChannel { get; set; }
        
        [Timestamp]
        public byte[]? Version { get; set; }
    }

    [PrimaryKey(nameof(Id))]
    public class LogSetting
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public Guid Id { get; set; }
        
        [Timestamp]
        public byte[]? Version { get; set; }
    }
    #endregion

    #region Application Settings
    [PrimaryKey(nameof(Id))]
    public class ApplicationSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public Guid Id { get; set; }

        [Required] public List<Application> Applications { get; set; } = [];
        
        [Timestamp]
        public byte[]? Version { get; set; }
    }

    [PrimaryKey(nameof(Id))]
    public class Application
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public Guid Id { get; set; }
        [Required] public required ApplicationSettings ApplicationSettings { get; set; }
        
        [Required] [MaxLength(Constants.EMBED_TITLE_CHARACTER_LIMIT)] public string Name { get; set; } = string.Empty;
        [Required] [MaxLength(Constants.EMBED_DESCRIPTION_CHARACTER_LIMIT)] public string AcceptMessage { get; set; } = string.Empty;
        [Required] public List<ApplicationQuestion> Questions { get; set; } = [];
        [Required] public List<ulong> AddRoles { get; set; } = [];
        [Required] public List<ulong> RemoveRoles { get; set; } = [];
        [Required] public byte Retries { get; set; }
        [Required] public ApplicationFailAction FailAction { get; set; }
        
        public ulong? SubmissionChannel { get; set; }
        
        [Timestamp]
        public byte[]? Version { get; set; }
    }

    [PrimaryKey(nameof(Id))]
    public class ApplicationQuestion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public Guid Id { get; set; }
        [Required] public required Application Application { get; set; }

        [Required] [MaxLength(Constants.EMBED_FIELD_NAME_CHARACTER_LIMIT)] public string Question { get; set; } = string.Empty;
        [Required] public int Index { get; set; }
        [Required] public bool Required { get; set; }
        
        [Timestamp]
        public byte[]? Version { get; set; }
    }
    #endregion
    
    #region Server Action
    [PrimaryKey(nameof(Id))]
    public class ServerActionsSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public Guid Id { get; set; }
        
        [Required] public List<ServerActionGroup> ActionGroups { get; set; } = [];
        
        [Timestamp]
        public byte[]? Version { get; set; }
    }

    [PrimaryKey(nameof(Id))]
    public class ServerActionGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public Guid Id { get; set; }
        
        [Required] public required ServerActionsSettings ServerActionsSettings { get; set; }
        
        [Required] public List<ServerAction> Actions { get; set; } = [];
        
        [Timestamp]
        public byte[]? Version { get; set; }
    }

    [PrimaryKey(nameof(Id))]
    public class ServerAction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public Guid Id { get; set; }
        
        [Required] public required ServerActionGroup ActionGroup { get; set; }
        
        [Required] public List<ulong> RequiredRoles { get; set; } = [];
        
        [Required] public GuildPermission RequiredPermissions { get; set; } = ulong.MinValue;
        
        [Timestamp]
        public byte[]? Version { get; set; }
    }
    #endregion

    public enum ApplicationFailAction
    {
        None,
        Kick,
        Ban,
        Timeout15Minutes,
        Timeout30Minutes,
        Timeout1Hour,
        Timeout6Hours,
        Timeout12Hours,
        Timeout1Day,
        Timeout1Week
    }
}
