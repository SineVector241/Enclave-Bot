using Discord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Enclave_Bot.Database
{
    public sealed class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Server> Servers { get; set; }

        public DatabaseContext()
        {
            try
            {
                ChangeTracker.LazyLoadingEnabled = false;
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                databaseCreator?.CreateTables();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}]: [Database] => {ex.Message}");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Config.BotConfiguration.SqlConnection);
            base.OnConfiguring(optionsBuilder);
        }

        public async Task CreateServerIfNotExistsAsync(IGuild guild)
        {
            if (!await Servers.AnyAsync(x => x.Id == guild.Id))
            {
                await Servers.AddAsync(new Server { Id = guild.Id });
                await SaveChangesAsync();
            }
        }

        public async Task CreateUserIfNotExistsAsync(IUser user)
        {
            if (!await Users.AnyAsync(x => x.Id == user.Id))
            {
                await Users.AddAsync(new User { Id = user.Id, Name = user.Username });
                await SaveChangesAsync();
            }
        }
    }
}
