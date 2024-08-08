namespace Enclave_Bot.Database
{
    public class User
    {
        public required ulong Id { get; set; }

        public ulong? XUID { get; set; }
        public string? DiscordUsername { get; set; }
        public string? MinecraftUsername { get; set; }

        public required DateTime LastActiveDC { get; set; } = DateTime.MinValue;
        public required DateTime LastActiveMC { get; set; } = DateTime.MinValue;
        public required byte ApplicationDenials { get; set; }
    }
}
