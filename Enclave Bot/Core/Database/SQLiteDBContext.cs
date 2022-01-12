using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Enclave_Bot.Core.Database
{
    public class SQLiteDBContext
    {
        public SQLiteConnection MyConnection;
        public SQLiteDBContext()
        {
            MyConnection = new SQLiteConnection(@"Data Source=Resources\Database.sqlite3");
            if(!File.Exists("Resources/Database.sqlite3"))
            {
                SQLiteConnection.CreateFile("Resources/Database.sqlite3");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Database Created");
            }
        }

        public void OpenConnection()
        {
            if (MyConnection.State != System.Data.ConnectionState.Open)
                MyConnection.Open();
        }

        public void CloseConnection()
        {
            if (MyConnection.State != System.Data.ConnectionState.Closed)
                MyConnection.Close();
        }
    }
}
