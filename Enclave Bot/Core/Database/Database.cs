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
            db.OpenConnection();
            cmd.Prepare();
            cmd.ExecuteNonQuery();
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
    }
}
