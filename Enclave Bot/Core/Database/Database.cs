using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Enclave_Bot.Core.Database
{
    public struct User
    {
        public ulong DiscordID { get; set; }
        public int Wallet { get; set; }
        public int Bank { get; set; }
        public int Level { get; set; }
        public int XP { get; set; }
    }

    public struct Settings
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

    public class Database
    {
        private SQLiteDBContext db = new SQLiteDBContext();
        public Database()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            string query = "CREATE TABLE IF NOT EXISTS users (id varchar(24), wallet int, bank int, level int, xp int)";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            string query2 = "CREATE TABLE IF NOT EXISTS settings (guildid varchar(24), loggingchannel varchar(24), welcomechannel varchar(24), fappchannel varchar(24), sappchannel varchar(24), parchmentcategory varchar(24), verifiedrole varchar(24), unverifiedrole varchar(24))";
            SQLiteCommand cmd2 = new SQLiteCommand(query2, db.MyConnection);
            db.OpenConnection();
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd2.Prepare();
            cmd2.ExecuteNonQuery();
            db.CloseConnection();
        }

        public void CreateAccount(User user)
        {
            string query = "INSERT INTO users (id,wallet,bank,level,xp) VALUES (@id,@wallet,@bank,@level,@xp)";
            db.OpenConnection();
            SQLiteCommand cmd = new SQLiteCommand(query,db.MyConnection);
            cmd.Parameters.AddWithValue("@id", user.DiscordID);
            cmd.Parameters.AddWithValue("@wallet", user.Wallet);
            cmd.Parameters.AddWithValue("@bank", user.Bank);
            cmd.Parameters.AddWithValue("@level", user.Level);
            cmd.Parameters.AddWithValue("@xp", user.XP);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            db.CloseConnection();
        }

        public void CreateSettings(Settings settings)
        {
            string query = "INSERT INTO settings (guildid,loggingchannel,welcomechannel,fappchannel,sappchannel,parchmentcategory,verifiedrole,unverifiedrole) VALUES (@guildid,@loggingchannel,@welcomechannel,@fappchannel,@sappchannel,@parchmentcategory,@verifiedrole,@unverifiedrole)";
            db.OpenConnection();
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            cmd.Parameters.AddWithValue("@guildid", settings.GuildID);
            cmd.Parameters.AddWithValue("@loggingchannel", settings.LoggingChannel);
            cmd.Parameters.AddWithValue("@welcomechannel", settings.WelcomeChannel);
            cmd.Parameters.AddWithValue("@fappchannel", settings.ApplicationChannel);
            cmd.Parameters.AddWithValue("@sappchannel", settings.StaffApplicationChannel);
            cmd.Parameters.AddWithValue("@parchmentcategory", settings.ParchmentCategory);
            cmd.Parameters.AddWithValue("@verifiedrole", settings.VerifiedRole);
            cmd.Parameters.AddWithValue("@unverifiedrole", settings.UnverifiedRole);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            db.CloseConnection();
        }

        public User GetUserByID(ulong id)
        {
            string query = $"SELECT * FROM users WHERE id = {id}";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            db.OpenConnection();
            cmd.Prepare();
            SQLiteDataReader result = cmd.ExecuteReader();
            User user = new User();
            if (result.HasRows) while (result.Read())
                {
                    user.DiscordID = id;
                    user.Wallet = Convert.ToInt32(result["wallet"].ToString());
                    user.Bank = Convert.ToInt32(result["bank"].ToString());
                    user.XP = Convert.ToInt32(result["xp"].ToString());
                    user.Level = Convert.ToInt32(result["level"].ToString());
                }
            return user;
        }

        public Settings GetGuildSettingsByID(ulong guildid)
        {
            string query = $"SELECT * FROM settings";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            db.OpenConnection();
            cmd.Prepare();
            SQLiteDataReader result = cmd.ExecuteReader();
            Settings settings = new Settings();
            if (result.HasRows) while (result.Read())
                {
                    settings.GuildID = guildid;
                    settings.LoggingChannel = (ulong)Convert.ToInt64(result["loggingchannel"]);
                    settings.WelcomeChannel = (ulong)Convert.ToInt64(result["welcomechannel"]);
                    settings.ApplicationChannel = (ulong)Convert.ToInt64(result["fappchannel"]);
                    settings.StaffApplicationChannel = (ulong)Convert.ToInt64(result["sappchannel"]);
                    settings.ParchmentCategory = (ulong)Convert.ToInt64(result["parchmentcategory"]);
                    settings.VerifiedRole = (ulong)Convert.ToInt64(result["verifiedrole"]);
                    settings.UnverifiedRole = (ulong)Convert.ToInt64(result["unverifiedrole"]);
                }
            return settings;
        }

        public bool HasAccount(ulong id)
        {
            string query = $"SELECT * FROM users WHERE id = {id}";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            db.OpenConnection();
            cmd.Prepare();
            SQLiteDataReader result = cmd.ExecuteReader();
            if (result.HasRows) return true;
            return false;
        }

        public bool HasSettings(ulong id)
        {
            string query = $"SELECT * FROM settings WHERE guildid = {id}";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            db.OpenConnection();
            cmd.Prepare();
            SQLiteDataReader result = cmd.ExecuteReader();
            if (result.HasRows) return true;
            return false;
        }
    }
}
