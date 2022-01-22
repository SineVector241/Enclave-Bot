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
        public ulong LoggingChannel { get; set; }
        public ulong WelcomeChannel { get; set; }
        public ulong ApplicationChannel { get; set; }
        public ulong StaffApplicationChannel { get; set; }
        public ulong VerifiedRole { get; set; }
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
            string query2 = "CREATE TABLE IF NOT EXISTS settings (loggingchannel varchar(24), welcomechannel varchar(24), fappchannel varchar(24), sappchannel varchar(24), verifiedlevel varchar(24))";
            SQLiteCommand cmd2 = new SQLiteCommand(query, db.MyConnection);
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

        public Settings GetSettings(Settings settings)
        {
            string query = $"SELECT * FROM settings";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            db.OpenConnection();
            cmd.Prepare();
            SQLiteDataReader result = cmd.ExecuteReader();
            Settings user = new Settings();
            if (result.HasRows) while (result.Read())
                {
                    settings.LoggingChannel = Convert.ToUInt32(result["loggingchannel"]);
                    settings.WelcomeChannel = Convert.ToUInt32(result["welcomechannel"]);
                    settings.ApplicationChannel = Convert.ToUInt32(result["fappchannel"]);
                    settings.StaffApplicationChannel = Convert.ToUInt32(result["sappchannel"]);
                    settings.VerifiedRole = Convert.ToUInt32(result["verifiedrole"]);
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

        public bool HasSettings()
        {
            string query = $"SELECT * FROM settings";
            SQLiteCommand cmd = new SQLiteCommand(query, db.MyConnection);
            db.OpenConnection();
            cmd.Prepare();
            SQLiteDataReader result = cmd.ExecuteReader();
            if (result.HasRows) return true;
            return false;
        }
    }
}
