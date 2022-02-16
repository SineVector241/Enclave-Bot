using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Enclave_Bot.Core.Database
{
    public struct GuildSettings
    {
        public ulong GuildID { get; set; }
        public ulong LoggingChannel { get; set; }
        public ulong WelcomeChannel { get; set; }
        public ulong ApplicationChannel { get; set; }
        public ulong StaffApplicationChannel { get; set; }
        public ulong ParchmentCategory { get; set; }
        public ulong VerifiedRole { get; set; }
        public ulong UnverifiedRole { get; set; }
    }

    public struct UserProfile
    {
        public ulong UserID { get; set; }
        public int Wallet { get; set; }
        public int Bank { get; set; }
        public int XP { get; set; }
        public int Level { get; set; }
    }

    public class Database
    {
        SQLiteDBContext db = new SQLiteDBContext();

        public Database()
        {
            CreateTable().GetAwaiter().GetResult();
        }

        private async Task CreateTable()
        {
            string query = "CREATE TABLE IF NOT EXISTS settings (ID varchar(18), LoggingChannel varchar(18), WelcomeChannel varchar(18), ApplicationChannel varchar(18), StaffApplicationChannel varchar(18), ParchmentCategory varchar(18), VerifiedRole varchar(18), UnverifiedRole varchar(18))";
            string query2 = "CREATE TABLE IF NOT EXISTS users (ID varchar(18), Wallet int, Bank int, XP int, Level int)";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            SQLiteCommand cmd2 = new SQLiteCommand(query2, db.MyConnection);
            cmd.Prepare();
            cmd2.Prepare();
            db.OpenConnection();
            cmd.ExecuteNonQuery();
            cmd2.ExecuteNonQuery();
            db.CloseConnection();

            await cmd.DisposeAsync();
            await cmd2.DisposeAsync();
        }

        //User Information
        public async Task CreateUserProfile(UserProfile profile)
        {
            string query = "INSERT INTO users (ID, Wallet, Bank, XP, Level) VALUES (@ID, @Wallet, @Bank, @XP, @Level)";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Parameters.AddWithValue("@ID", profile.UserID);
            cmd.Parameters.AddWithValue("@Wallet",profile.Wallet);
            cmd.Parameters.AddWithValue("@Bank",profile.Bank);
            cmd.Parameters.AddWithValue("@XP", profile.XP);
            cmd.Parameters.AddWithValue("@Level", profile.Level);
            cmd.Prepare();
            db.OpenConnection();
            cmd.ExecuteNonQuery();
            db.CloseConnection();
            await cmd.DisposeAsync();
        }

        public async Task UpdateUserProfile(UserProfile profile)
        {
            string query = $"UPDATE users SET Wallet = {profile.Wallet}, Bank = {profile.Bank}, XP = {profile.XP}, Level = {profile.Level} WHERE ID = {profile.UserID}";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Prepare();
            db.OpenConnection();
            cmd.ExecuteNonQuery();
            db.CloseConnection();
            await cmd.DisposeAsync();
        }

        public async Task<UserProfile> GetUserProfileById(ulong id)
        {
            string query = $"SELECT * FROM users WHERE ID = {id}";
            SQLiteCommand cmd = new SQLiteCommand(query,db.MyConnection);
            cmd.Prepare();
            db.OpenConnection();
            SQLiteDataReader result = cmd.ExecuteReader();
            UserProfile profile = new UserProfile();
            if(result.HasRows) while (result.Read())
                {
                    profile.UserID = id;
                    profile.Wallet = Convert.ToInt32(result["Wallet"]);
                    profile.Bank = Convert.ToInt32(result["Bank"]);
                    profile.XP = Convert.ToInt32(result["XP"]);
                    profile.Level = Convert.ToInt32(result["Level"]);
                }
            db.CloseConnection();
            await cmd.DisposeAsync();
            await result.DisposeAsync();
            return profile;
        }

        public async Task<bool> UserHasProfile(ulong id)
        {
            string query = $"SELECT * FROM users WHERE ID = {id}";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Prepare();
            db.OpenConnection();
            SQLiteDataReader result = cmd.ExecuteReader();
            bool hasProfile = false;
            if (result.HasRows) hasProfile = true;
            db.CloseConnection();
            await cmd.DisposeAsync();
            await result.DisposeAsync();
            return hasProfile;
        }

        //Guild Information
        public async Task CreateGuildSettings(GuildSettings settings)
        {
            string query = "INSERT INTO settings (ID, LoggingChannel, WelcomeChannel, ApplicationChannel, StaffApplicationChannel, ParchmentCategory, VerifiedRole, UnverifiedRole) VALUES (@ID, @LoggingChannel, @WelcomeChannel, @ApplicationChannel, @StaffApplicationChannel, @ParchmentCategory, @VerifiedRole, @UnverifiedRole)";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Parameters.AddWithValue("@ID", settings.GuildID);
            cmd.Parameters.AddWithValue("@LoggingChannel", settings.LoggingChannel);
            cmd.Parameters.AddWithValue("@WelcomeChannel", settings.WelcomeChannel);
            cmd.Parameters.AddWithValue("@ApplicationChannel", settings.ApplicationChannel);
            cmd.Parameters.AddWithValue("@StaffApplicationChannel", settings.StaffApplicationChannel);
            cmd.Parameters.AddWithValue("@ParchmentCategory", settings.ParchmentCategory);
            cmd.Parameters.AddWithValue("@VerifiedRole", settings.VerifiedRole);
            cmd.Parameters.AddWithValue("@UnverifiedRole", settings.UnverifiedRole);
            cmd.Prepare();
            db.OpenConnection();
            cmd.ExecuteNonQuery();
            db.CloseConnection();
            await cmd.DisposeAsync();
        }

        public async Task EditGuildSettings(GuildSettings settings)
        {
            string query = $"UPDATE settings SET LoggingChannel = {settings.LoggingChannel}, WelcomeChannel = {settings.WelcomeChannel}, ApplicationChannel = {settings.ApplicationChannel}, StaffApplicationChannel = {settings.StaffApplicationChannel}, ParchmentCategory = {settings.ParchmentCategory}, VerifiedRole = {settings.VerifiedRole}, UnverifiedRole = {settings.UnverifiedRole} WHERE ID = {settings.GuildID}";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Prepare();
            db.OpenConnection();
            cmd.ExecuteNonQuery();
            db.CloseConnection();
            await cmd.DisposeAsync();
        }

        public async Task<GuildSettings> GetGuildSettingsById(ulong id)
        {
            string query = $"SELECT * FROM settings WHERE ID = {id}";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Prepare();
            db.OpenConnection();
            SQLiteDataReader result = cmd.ExecuteReader();
            GuildSettings settings = new GuildSettings();
            if (result.HasRows) while (result.Read())
                {
                    settings.GuildID = id;
                    settings.LoggingChannel = (ulong)Convert.ToInt64(result["LoggingChannel"]);
                    settings.WelcomeChannel = (ulong)Convert.ToInt64(result["WelcomeChannel"]);
                    settings.ApplicationChannel = (ulong)Convert.ToInt64(result["ApplicationChannel"]);
                    settings.StaffApplicationChannel = (ulong)Convert.ToInt64(result["StaffApplicationChannel"]);
                    settings.ParchmentCategory = (ulong)Convert.ToInt64(result["ParchmentCategory"]);
                    settings.VerifiedRole = (ulong)Convert.ToInt64(result["VerifiedRole"]);
                    settings.UnverifiedRole = (ulong)Convert.ToInt64(result["UnverifiedRole"]);
                }
            db.CloseConnection();
            await cmd.DisposeAsync();
            await result.DisposeAsync();
            return settings;
        }

        public async Task<bool> GuildHasSettings(ulong id)
        {
            string query = $"SELECT * FROM settings WHERE ID = {id}";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Prepare();
            db.OpenConnection();
            SQLiteDataReader result = cmd.ExecuteReader();
            bool hasSettings = false;
            if (result.HasRows) hasSettings = true;
            db.CloseConnection();
            await cmd.DisposeAsync();
            await result.DisposeAsync();
            return hasSettings;
        }
    }
}
