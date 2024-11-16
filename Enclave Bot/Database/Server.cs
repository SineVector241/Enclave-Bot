using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enclave_Bot.Database
{
    [PrimaryKey(nameof(Id))]
    public class Server
    {
        [Required]
        public required ulong Id { get; set; }

        [Required]
        public LogSettings LogSettings { get; set; }
        [Required]
        public ApplicationSettings ApplicationSettings { get; set; }
        [Required]
        public ICollection<ServerAction> ServerActions { get; set; }

        public Server()
        {
            LogSettings = new LogSettings() { Id = Id };
            ApplicationSettings = new ApplicationSettings() { Id = Id };
            ServerActions = new List<ServerAction>();
        }
    }

    #region Log Settings
    [PrimaryKey(nameof(Id))]
    public class LogSettings
    {
        [Required]
        public required ulong Id { get; set; }

        public ulong? DefaultChannel { get; set; }
        [Required]
        public ICollection<LogSetting> Settings { get; set; }

        public LogSettings()
        {
            Settings = new List<LogSetting>();
        }
    }

    public class LogSetting
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required Guid LogSettingsId { get; set; }
    }
    #endregion

    #region Application Settings
    [PrimaryKey(nameof(Id))]
    public class ApplicationSettings
    {
        [Required]
        public required ulong Id { get; set; }

        [Required]
        public ICollection<Application> Applications { get; set; }

        public ApplicationSettings()
        {
            Applications = new List<Application>();
        }
    }

    public class Application
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required ulong ApplicationSettingsId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public ICollection<ApplicationQuestion> Questions { get; set; }
        public ServerAction? OnAccept { get; set; }
        public ServerAction? OnDeny { get; set; }
        public ServerAction? OnSubmit { get; set; }

        public Application()
        {
            Name = string.Empty;
            Questions = new List<ApplicationQuestion>();
        }
    }

    [Index(nameof(Index), IsUnique = true)]
    public class ApplicationQuestion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required Guid ApplicationId { get; set; }

        [Required]
        public string Question { get; set; }
        [Required]
        public int Index { get; set; }
        [Required]
        public bool Required { get; set; }
        public ServerAction? OnEntered { get; set; }

        public ApplicationQuestion()
        {
            Question = string.Empty;
            Index = 0;
            Required = false;
        }
    }
    #endregion

    #region Server Actions
    public class ServerAction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required ulong ServerId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public ICollection<ActionBehavior> Behaviors { get; set; }

        public ServerAction()
        {
            Name = string.Empty;
            Behaviors = new List<ActionBehavior>();
        }
    }

    public class ActionBehavior
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required Guid ServerActionId { get; set; }

        [Required]
        public ActionBehaviorType Type { get; set; }
        [Required]
        public string Data { get; set; }
        [Required]
        public ICollection<ActionCondition> Conditions { get; set; }

        public ActionBehavior()
        {
            Type = ActionBehaviorType.AddRole;
            Data = string.Empty;
            Conditions = new List<ActionCondition>();
        }
    }

    public class ActionCondition
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required Guid ActionBehaviorId { get; set; }

        [Required]
        public ActionConditionType ConditionType { get; set; }

        public ActionCondition()
        {
            ConditionType = ActionConditionType.HasRole;
        }
    }
    #endregion

    public enum ServerLogType
    {

    }

    public enum ActionBehaviorType
    {
        AddRole,
        RemoveRole,
        SendApplication
    }

    public enum ActionConditionType
    {
        HasRole
    }
}
