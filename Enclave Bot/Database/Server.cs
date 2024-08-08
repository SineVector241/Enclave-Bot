using System.ComponentModel.DataAnnotations.Schema;

namespace Enclave_Bot.Database
{
    public class Server
    {
        public required ulong Id { get; set; }

        public required ServerLogSettings LogSettings { get; set; }
        public ICollection<ServerAction> ServerActions { get; set; } = [];
        public ICollection<ServerApplication> Applications { get; set; } = [];
    }

    public class ServerLogSettings
    {
        public required ulong Id { get; set; }
    }

    public class ServerAction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public required ulong ServerId { get; set; }
        public required Server Server { get; set; }

        public required string Name { get; set; }
        public ICollection<ServerBehaviorCondition> Conditions { get; set; } = [];
        public ICollection<ServerBehavior> Behaviors { get; set; } = [];
    }

    public class ServerApplication
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public required ulong ServerId { get; set; }
        public required Server Server { get; set; }

        public required string Name { get; set; }
        public required List<string> Questions { get; set; }
    }

    public class ServerBehavior
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public required Guid ServerActionsId { get; set; }
        public required ServerAction ServerActions { get; set; }

        public BehaviorType Type { get; set; }
        public string? Data { get; set; }
    }

    public class ServerBehaviorCondition
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public required Guid ServerActionsId { get; set; }
        public required ServerAction ServerActions { get; set; }

        public BehaviorConditionType Type { get; set; }
        public string? Data { get; set; }
    }

    public enum BehaviorType
    {
        AddRole,
        RemoveRole,
        SendApplication
    }

    public enum BehaviorConditionType
    {
        HasRole,
        HasNoRole
    }
}
