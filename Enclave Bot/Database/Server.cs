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
    public class LogSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required ulong ServerId { get; set; }

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
    public class ApplicationSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public required ulong ServerId { get; set; }

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
        public required Guid ApplicationSettingsId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public ICollection<ApplicationQuestion> Questions { get; set; }

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

        public ApplicationQuestion()
        {
            Question = string.Empty;
            Index = 0;
            Required = false;
        }
    }
    #endregion

    public enum ServerLogType
    {

    }
}
