using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Enclave_Bot.Database
{
    [PrimaryKey(nameof(Id))]
    public class User
    {
        [Required]
        public required ulong Id { get; set; }

        [Required] [MaxLength(100)] public string Name { get; set; } = string.Empty;
        [Required] public DateTime LastActive { get; set; } = DateTime.UtcNow;
    }
}
