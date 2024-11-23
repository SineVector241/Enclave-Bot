using Discord;
using Discord.WebSocket;
using Enclave_Bot.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Enclave_Bot.Database
{
    public sealed class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Server> Servers { get; set; }

        public DbSet<LogSettings> ServerLogSettings { get; set; }
        public DbSet<LogSetting> ServerLogsSettings { get; set; }

        public DbSet<ApplicationSettings> ServerApplicationSettings { get; set; }
        public DbSet<Application> ServerApplications { get; set; }
        public DbSet<ApplicationQuestion> ServerApplicationQuestions { get; set; }

        public DatabaseContext()
        {
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                databaseCreator?.CreateTables();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}]: [Database] => {ex.Message}");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(Config.BotConfiguration.SqlConnection);

        public async Task<User> GetOrCreateUserById(ulong id)
        {
            var user = await Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != null) return user;
            
            user = new User() { Id = id, Username = string.Empty, LastActive = DateTime.UtcNow };
            Users.Add(user);

            await SaveChangesAsync();
            return user;
        }

        public async Task<Server> GetOrCreateServerById(ulong id)
        {
            var server = await Servers.FirstOrDefaultAsync(x => x.Id == id);
            if (server != null) return server;

            server = new Server() { Id = id };
            server.ApplicationSettings = new ApplicationSettings() { ServerId = server.Id };
            server.LogSettings = new LogSettings() { ServerId = server.Id, Settings = new List<LogSetting>() };
            Servers.Add(server);

            await SaveChangesAsync();
            return server;
        }

        public async Task<Application[]> GetServerApplications(ulong serverId)
        {
            var server = await GetOrCreateServerById(serverId);
            var serverApplicationSettings = await ServerApplicationSettings.FirstAsync(x => x.ServerId == server.Id);
            return await ServerApplications.Where(x => x.ApplicationSettingsId == serverApplicationSettings.Id).ToArrayAsync();
        }

        public async Task<Application?> GetApplicationById(ulong serverId, Guid applicationId)
        {
            var server = await GetOrCreateServerById(serverId);
            var serverApplicationSettings = await ServerApplicationSettings.FirstAsync(x => x.ServerId == server.Id);
            
            return await ServerApplications.FirstOrDefaultAsync(x => x.ApplicationSettingsId == serverApplicationSettings.Id && x.Id == applicationId);
        }
    }
}
