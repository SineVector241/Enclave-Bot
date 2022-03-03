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
        public ulong BountyChannel { get; set; }
        public ulong ParchmentCategory { get; set; }
        public ulong VerifiedRole { get; set; }
        public ulong UnverifiedRole { get; set; }
        public ulong StaffRole { get; set; }
        public string WelcomeMessage { get; set; }
        public string LeaveMessage { get; set; }
    }

    public struct UserProfile
    {
        public ulong UserID { get; set; }
        public int Wallet { get; set; }
        public int Bank { get; set; }
        public int XP { get; set; }
        public int Level { get; set; }
        public string WorkType { get; set; }
    }

    public struct ActivityData
    {
        public ulong ID { get; set; }
        public ulong RoleID { get; set; }
        public int TimeDays { get; set; }
        public int Action { get; set; }
        public ulong RemoveRole1 { get; set; }
        public ulong RemoveRole2 { get; set; }
        public ulong RemoveRole3 { get; set; }
        public ulong RemoveRole4 { get; set; }
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
            string query = "CREATE TABLE IF NOT EXISTS settings (ID varchar(18), LoggingChannel varchar(18), WelcomeChannel varchar(18), ApplicationChannel varchar(18), StaffApplicationChannel varchar(18), BountyChannel varchar(18), ParchmentCategory varchar(18), VerifiedRole varchar(18), UnverifiedRole varchar(18), StaffRole varchar(18), WelcomeMessage string, LeaveMessage string)";
            string query2 = "CREATE TABLE IF NOT EXISTS users (ID varchar(18), Wallet int, Bank int, XP int, Level int, WorkType string)";
            string query3 = "CREATE TABLE IF NOT EXISTS activitychecker (ID varchar(18), RoleID varchar(18), TimeDays int, Action int, RemoveRole1 varchar(18), RemoveRole2 varchar(18), RemoveRole3 varchar(18), RemoveRole4 varchar(18))";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            SQLiteCommand cmd2 = new SQLiteCommand(query2, db.MyConnection);
            SQLiteCommand cmd3 = new SQLiteCommand(query3, db.MyConnection);
            cmd.Prepare();
            cmd2.Prepare();
            cmd3.Prepare();
            db.OpenConnection();
            cmd.ExecuteNonQuery();
            cmd2.ExecuteNonQuery();
            cmd3.ExecuteNonQuery();
            db.CloseConnection();

            await cmd.DisposeAsync();
            await cmd2.DisposeAsync();
            await cmd3.DisposeAsync();
        }

        //User Information
        public async Task CreateUserProfile(UserProfile profile)
        {
            string query = "INSERT INTO users (ID, Wallet, Bank, XP, Level, WorkType) VALUES (@ID, @Wallet, @Bank, @XP, @Level, @WorkType)";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Parameters.AddWithValue("@ID", profile.UserID);
            cmd.Parameters.AddWithValue("@Wallet", profile.Wallet);
            cmd.Parameters.AddWithValue("@Bank", profile.Bank);
            cmd.Parameters.AddWithValue("@XP", profile.XP);
            cmd.Parameters.AddWithValue("@Level", profile.Level);
            cmd.Parameters.AddWithValue("@WorkType", profile.WorkType);
            cmd.Prepare();
            db.OpenConnection();
            cmd.ExecuteNonQuery();
            db.CloseConnection();
            await cmd.DisposeAsync();
        }

        public async Task UpdateUserProfile(UserProfile profile)
        {
            string query = $"UPDATE users SET Wallet = {profile.Wallet}, Bank = {profile.Bank}, XP = {profile.XP}, Level = {profile.Level}, WorkType = \"{profile.WorkType}\" WHERE ID = {profile.UserID}";
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
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Prepare();
            db.OpenConnection();
            SQLiteDataReader result = cmd.ExecuteReader();
            UserProfile profile = new UserProfile();
            if (result.HasRows) while (result.Read())
                {
                    profile.UserID = id;
                    profile.Wallet = Convert.ToInt32(result["Wallet"]);
                    profile.Bank = Convert.ToInt32(result["Bank"]);
                    profile.XP = Convert.ToInt32(result["XP"]);
                    profile.Level = Convert.ToInt32(result["Level"]);
                    profile.WorkType = result["WorkType"].ToString();
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
            string query = "INSERT INTO settings (ID, LoggingChannel, WelcomeChannel, ApplicationChannel, StaffApplicationChannel, BountyChannel, ParchmentCategory, VerifiedRole, UnverifiedRole, StaffRole, WelcomeMessage, LeaveMessage) VALUES (@ID, @LoggingChannel, @WelcomeChannel, @ApplicationChannel, @StaffApplicationChannel, @BountyChannel, @ParchmentCategory, @VerifiedRole, @UnverifiedRole, @StaffRole, @WelcomeMessage, @LeaveMessage)";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Parameters.AddWithValue("@ID", settings.GuildID);
            cmd.Parameters.AddWithValue("@LoggingChannel", settings.LoggingChannel);
            cmd.Parameters.AddWithValue("@WelcomeChannel", settings.WelcomeChannel);
            cmd.Parameters.AddWithValue("@ApplicationChannel", settings.ApplicationChannel);
            cmd.Parameters.AddWithValue("@StaffApplicationChannel", settings.StaffApplicationChannel);
            cmd.Parameters.AddWithValue("@BountyChannel", settings.BountyChannel);
            cmd.Parameters.AddWithValue("@ParchmentCategory", settings.ParchmentCategory);
            cmd.Parameters.AddWithValue("@VerifiedRole", settings.VerifiedRole);
            cmd.Parameters.AddWithValue("@UnverifiedRole", settings.UnverifiedRole);
            cmd.Parameters.AddWithValue("@StaffRole", settings.StaffRole);
            cmd.Parameters.AddWithValue("@WelcomeMessage", settings.WelcomeMessage);
            cmd.Parameters.AddWithValue("LeaveMessage", settings.LeaveMessage);
            cmd.Prepare();
            db.OpenConnection();
            cmd.ExecuteNonQuery();
            db.CloseConnection();
            await cmd.DisposeAsync();
        }

        public async Task EditGuildSettings(GuildSettings settings)
        {
            string query = $"UPDATE settings SET LoggingChannel = {settings.LoggingChannel}, WelcomeChannel = {settings.WelcomeChannel}, ApplicationChannel = {settings.ApplicationChannel}, StaffApplicationChannel = {settings.StaffApplicationChannel}, BountyChannel = {settings.BountyChannel}, ParchmentCategory = {settings.ParchmentCategory}, VerifiedRole = {settings.VerifiedRole}, UnverifiedRole = {settings.UnverifiedRole}, StaffRole = {settings.StaffRole}, WelcomeMessage = \"{settings.WelcomeMessage}\", LeaveMessage = \"{settings.LeaveMessage}\" WHERE ID = {settings.GuildID}";
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
                    settings.BountyChannel = (ulong)Convert.ToInt64(result["BountyChannel"]);
                    settings.ParchmentCategory = (ulong)Convert.ToInt64(result["ParchmentCategory"]);
                    settings.VerifiedRole = (ulong)Convert.ToInt64(result["VerifiedRole"]);
                    settings.UnverifiedRole = (ulong)Convert.ToInt64(result["UnverifiedRole"]);
                    settings.StaffRole = (ulong)Convert.ToInt64(result["StaffRole"]);
                    settings.WelcomeMessage = result["WelcomeMessage"].ToString();
                    settings.LeaveMessage = result["LeaveMessage"].ToString();
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

        public async Task CreateActivity(ActivityData activity)
        {
            string query = "INSERT INTO activitychecker (ID, RoleID, TimeDays, Action, RemoveRole1, RemoveRole2, RemoveRole3, RemoveRole4) VALUES (@ID, @RoleID, @TimeDays, @Action, @RemoveRole1, @RemoveRole2, @RemoveRole3, @RemoveRole4)";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Parameters.AddWithValue("@ID", activity.ID);
            cmd.Parameters.AddWithValue("@RoleID", activity.RoleID);
            cmd.Parameters.AddWithValue("@TimeDays", activity.TimeDays);
            cmd.Parameters.AddWithValue("@Action", activity.Action);
            cmd.Parameters.AddWithValue("@RemoveRole1", activity.RemoveRole1);
            cmd.Parameters.AddWithValue("@RemoveRole2", activity.RemoveRole2);
            cmd.Parameters.AddWithValue("@RemoveRole3", activity.RemoveRole3);
            cmd.Parameters.AddWithValue("@RemoveRole4", activity.RemoveRole4);
            cmd.Prepare();
            db.OpenConnection();
            cmd.ExecuteNonQuery();
            db.CloseConnection();
            await cmd.DisposeAsync();
        }

        public async Task UpdateActivity(ActivityData activity)
        {
            string query = $"UPDATE activitychecker SET ID = {activity.ID}, RoleID = {activity.RoleID}, TimeDays = {activity.TimeDays}, Action = {activity.Action}, RemoveRole1 = {activity.RemoveRole1}, RemoveRole2 = {activity.RemoveRole2}, RemoveRole3 = {activity.RemoveRole3}, RemoveRole4 = {activity.RemoveRole4} WHERE ID = {activity.ID} AND RoleID = {activity.RoleID}";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Prepare();
            db.OpenConnection();
            cmd.ExecuteNonQuery();
            db.CloseConnection();
            await cmd.DisposeAsync();
        }

        public async Task<ActivityData> GetActivity(ulong id, ulong roleID)
        {
            string query = $"SELECT * FROM activitychecker WHERE ID = {id} AND RoleID = {roleID}";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Prepare();
            db.OpenConnection();
            SQLiteDataReader result = cmd.ExecuteReader();
            ActivityData activity = new ActivityData();
            if (result.HasRows) while (result.Read())
                {
                    activity.ID = id;
                    activity.RoleID = roleID;
                    activity.TimeDays = Convert.ToInt16(result["TimeDays"]);
                    activity.Action = Convert.ToInt16(result["TimeDays"]);
                    activity.RemoveRole1 = (ulong)Convert.ToInt64(result["RemoveRole1"]);
                    activity.RemoveRole2 = (ulong)Convert.ToInt64(result["RemoveRole2"]);
                    activity.RemoveRole3 = (ulong)Convert.ToInt64(result["RemoveRole3"]);
                    activity.RemoveRole4 = (ulong)Convert.ToInt64(result["RemoveRole4"]);
                }
            db.CloseConnection();
            await cmd.DisposeAsync();
            await result.DisposeAsync();
            return activity;
        }
    }
}
