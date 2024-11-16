using System.ComponentModel.DataAnnotations;

namespace Enclave_Bot.Database
{
    public class User
    {
        [Required]
        public required ulong Id { get; set; }

        [Required]
        public string Username { get; set; }
        [Required]
        public DateTime LastActive { get; set; }

        public User()
        {
            Username = string.Empty;
            LastActive = DateTime.MinValue;
        }
    }
}
