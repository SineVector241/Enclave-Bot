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
        public DbSet<ServerApplication> ServerApplications { get; set; }
        public DbSet<ServerAction> ServerActions { get; set; }
        public DbSet<ServerBehaviorCondition> ServerActionConditions { get; set; }
        public DbSet<ServerBehavior> ServerActionBehaviors { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            try
            {
                var databaseCreator = (Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator);
                databaseCreator?.CreateTables();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}]: [Database] => {ex.Message}");
            }
        }

        public async Task<User> GetOrCreateUserById(ulong id, SocketInteraction? context = null, bool ephemeral = false, RequestOptions? options = null)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);
            if (user != null) return user;
            
            user = new User() { Id = id, ApplicationDenials = 0, LastActiveDC = DateTime.UtcNow, LastActiveMC = DateTime.MinValue };
            Users.Add(user);
            if (context != null)
                await context.DeferSafelyAsync(ephemeral, options);

            await SaveChangesAsync();
            return user;
        }

        public async Task<Server> GetOrCreateServerById(ulong id, SocketInteraction? context = null, bool ephemeral = false, RequestOptions? options = null)
        {
            var server = Servers.FirstOrDefault(x => x.Id == id);
            if (server != null) return server;
            
            server = new Server() { Id = id, LogSettings = new ServerLogSettings() { Id = id } };
            Servers.Add(server);
            if (context != null)
                await context.DeferSafelyAsync(ephemeral, options);

            await SaveChangesAsync();
            return server;
        }
    }
}
