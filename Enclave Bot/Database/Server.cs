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

        public Server()
        {
            LogSettings = new LogSettings() { ServerId = Id };
            ApplicationSettings = new ApplicationSettings() { ServerId = Id };
        }
    }

    #region Log Settings
    public class LogSettings(ICollection<LogSetting> settings)
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required ulong ServerId { get; set; }

        public ulong? DefaultChannel { get; set; }
        [Required]
        public ICollection<LogSetting> Settings { get; set; } = settings;

        public LogSettings() : this(new List<LogSetting>())
        {
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
    public class ApplicationSettings(ICollection<Application> applications)
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required ulong ServerId { get; set; }

        [Required]
        public ICollection<Application> Applications { get; set; } = applications;

        public ApplicationSettings() : this(new List<Application>())
        {
        }
    }

    public class Application(
        string name,
        string acceptMessage,
        ICollection<ApplicationQuestion> questions,
        List<ulong> addRoles,
        List<ulong> removeRoles)
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        [Required]
        public required Guid ApplicationSettingsId { get; set; }

        [Required]
        public string Name { get; set; } = name;
        
        [Required]
        public string AcceptMessage { get; set; } = acceptMessage;
        
        public ulong? SubmissionChannel { get; set; }
        
        [Required]
        public ICollection<ApplicationQuestion> Questions { get; set; } = questions;

        [Required]
        public List<ulong> AddRoles { get; set; } = addRoles;

        [Required]
        public List<ulong> RemoveRoles { get; set; } = removeRoles;

        [Required]
        public byte Retries { get; set; }
        [Required]
        public ApplicationFailAction FailAction { get; set; }

        public Application() : this(string.Empty, string.Empty, new List<ApplicationQuestion>(), [], [])
        {
            Retries = 0;
            FailAction = ApplicationFailAction.None;
        }
    }

    [Index(nameof(Index), nameof(ApplicationId), IsUnique = true)]
    public class ApplicationQuestion(string question)
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required Guid ApplicationId { get; set; }

        [Required]
        public string Question { get; set; } = question;

        [Required]
        public int Index { get; set; }
        [Required]
        public bool Required { get; set; }

        public ApplicationQuestion() : this(string.Empty)
        {
            Index = 0;
            Required = false;
        }
    }
    #endregion

    public enum ServerLogType
    {

    }

    public enum ApplicationFailAction
    {
        None,
        Kick,
        Ban
    }
}
