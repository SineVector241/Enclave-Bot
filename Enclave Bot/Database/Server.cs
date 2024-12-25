using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Required] public ActionsSettings ActionsSettings { get; init; } = new();
        
        [Timestamp]
        public byte[]? Version { get; init; }
    }
    
    #region Server Settings
    [PrimaryKey(nameof(Id))]
    public class ServerSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public long Id { get; set; }
        
        [Required] [MaxLength(Constants.SELECT_MENU_OPTIONS_LIMIT)] public List<ulong> StaffRoles { get; set; } = [];
        
        [Timestamp]
        public byte[]? Version { get; init; }
    }
    #endregion

    #region Log Settings
    [PrimaryKey(nameof(Id))]
    public class LogSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public long Id { get; set; } 
        
        [Required] public List<LogSetting> Settings { get; set; } = [];
        
        public ulong? DefaultChannel { get; set; }
        
        [Timestamp]
        public byte[]? Version { get; init; }
    }

    [PrimaryKey(nameof(Id))]
    public class LogSetting
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public long Id { get; set; }
        
        [Timestamp]
        public byte[]? Version { get; init; }
    }
    #endregion

    #region Application Settings
    [PrimaryKey(nameof(Id))]
    public class ApplicationSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public long Id { get; set; }

        [Required] public List<Application> Applications { get; set; } = [];
        
        [Timestamp]
        public byte[]? Version { get; init; }
    }

    [PrimaryKey(nameof(Id))]
    public class Application
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public long Id { get; set; }
        [Required] public required ApplicationSettings ApplicationSettings { get; set; }
        
        [Required] [MaxLength(Constants.EMBED_TITLE_CHARACTER_LIMIT)] public string Name { get; set; } = string.Empty;
        [Required] [MaxLength(Constants.EMBED_DESCRIPTION_CHARACTER_LIMIT)] public string AcceptMessage { get; set; } = string.Empty;
        [Required] public List<ApplicationQuestion> Questions { get; set; } = [];
        [Required] [MaxLength(Constants.SELECT_MENU_OPTIONS_LIMIT)] public List<ulong> AddRoles { get; set; } = [];
        [Required] [MaxLength(Constants.SELECT_MENU_OPTIONS_LIMIT)] public List<ulong> RemoveRoles { get; set; } = [];
        [Required] public byte Retries { get; set; }
        [Required] public ApplicationFailAction FailAction { get; set; }
        
        public ulong? SubmissionChannel { get; set; }
        
        [Timestamp]
        public byte[]? Version { get; init; }
    }

    [PrimaryKey(nameof(Id))]
    public class ApplicationQuestion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public long Id { get; set; }
        [Required] public required Application Application { get; set; }

        [Required] [MaxLength(Constants.EMBED_FIELD_NAME_CHARACTER_LIMIT)] public string Question { get; set; } = string.Empty;
        [Required] public int Index { get; set; }
        [Required] public bool Required { get; set; }
        
        [Timestamp]
        public byte[]? Version { get; init; }
    }
    #endregion
    
    #region Server Action
    [PrimaryKey(nameof(Id))]
    public class ActionsSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public long Id { get; set; }
        
        [Required] public List<ActionGroup> ActionGroups { get; set; } = [];
        
        [Timestamp]
        public byte[]? Version { get; init; }
    }

    [PrimaryKey(nameof(Id))]
    public class ActionGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public long Id { get; set; }
        
        [Required] [MaxLength(Constants.EMBED_TITLE_CHARACTER_LIMIT)] public string Name { get; set; } = string.Empty;
        [Required] public required ActionsSettings ActionsSettings { get; set; }
        [Required] public List<ServerAction> Actions { get; set; } = [];
        
        [Timestamp]
        public byte[]? Version { get; init; }
    }

    [PrimaryKey(nameof(Id))]
    public class ServerAction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Required] public long Id { get; set; }
        
        [Required] public required ActionGroup ActionGroup { get; set; }

        [Required] public ActionType Type { get; set; } = ActionType.AddRoles;
        [Required] [MaxLength(Constants.EMBED_TITLE_CHARACTER_LIMIT)] public string Name { get; set; } = string.Empty;
        
        [Required] [MaxLength(Constants.SELECT_MENU_OPTIONS_LIMIT)] public List<ulong> AllOfRoles { get; set; } = [];
        
        [Required] [MaxLength(Constants.SELECT_MENU_OPTIONS_LIMIT)] public List<ulong> AnyOfRoles { get; set; } = [];
        
        [Required] [MaxLength(Constants.SELECT_MENU_OPTIONS_LIMIT)] public List<ulong> NoneOfRoles { get; set; } = [];
        
        [MaxLength(1000)] public string? Data { get; set; }
        
        [Timestamp]
        public byte[]? Version { get; init; }
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

    public enum ActionType
    {
        AddRoles,
        RemoveRoles,
        SendApplication
    }
}
