using Newtonsoft.Json;

namespace Enclave_Bot.Core.Database
{
    public class Users
    {
        private const string _dbFolder = "Resources";
        private const string _usersFile = "users.json";
        public static Structure Current { get; set; } = new Structure();
        static Users()
        {
            try
            {
                if (!Directory.Exists(_dbFolder))
                {
                    Directory.CreateDirectory(_dbFolder);
                }
                else if (!File.Exists(_dbFolder + "/" + _usersFile))
                {
                    string botConfigJson = JsonConvert.SerializeObject(Current, Formatting.Indented);
                    File.WriteAllText(_dbFolder + "/" + _usersFile, botConfigJson);
                }
                else
                {
                    string users = File.ReadAllText(_dbFolder + "/" + _usersFile);
                    var loaded = JsonConvert.DeserializeObject<Structure>(users);
                    if (loaded != null)
                        Current = loaded;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}]: [ERROR] => An error occured in Users.cs \nError Info:\n{ex}");
            }
        }

        public static User? GetUserById(ulong id)
        {
            CreateUserIfNotExists(id);
            var user = Current.Users.FirstOrDefault(x => x.Id == id);
            if(user != null)
                return user;
            return null;
        }

        public static void UpdateUser(User user)
        {
            for(int i = 0; i < Current.Users.Count; i++)
            {
                if (Current.Users[i].Id == user.Id)
                {
                    Current.Users[i] = user;
                    UpdateUsers();
                    break;
                }
            }
        }

        public static void CreateUserIfNotExists(ulong id)
        {
            var user = new User { Id = id };
            if (!Current.Users.Exists(x => x.Id == id))
            {
                Current.Users.Add(user);
                UpdateUsers();
            }
        }

        public static void ReloadUsers()
        {
            if (!Directory.Exists(_dbFolder))
            {
                Directory.CreateDirectory(_dbFolder);
            }
            else if (!File.Exists(_dbFolder + "/" + _usersFile))
            {
                string usersJson = JsonConvert.SerializeObject(Current, Formatting.Indented);
                File.WriteAllText(_dbFolder + "/" + _usersFile, usersJson);
            }
            else
            {
                string users = File.ReadAllText(_dbFolder + "/" + _usersFile);
                var loaded = JsonConvert.DeserializeObject<Structure>(users);
                if (loaded != null)
                    Current = loaded;
                else
                    throw new Exception("Could not reload settings!");
            }
        }

        public static void UpdateUsers()
        {
            string usersJson = JsonConvert.SerializeObject(Current, Formatting.Indented);
            File.WriteAllText(_dbFolder + "/" + _usersFile, usersJson);
        }

        public class Structure
        {
            public List<User> Users { get; set; } = new List<User>();
        }

        public class User
        {
            public ulong Id { get; set; }
            public string Gamertag { get; set; } = "";
            public string LastActiveUnix { get; set; } = "";

            public int Wallet { get; set; }
            public int Bank { get; set; }
            public int ApplicationDenials { get; set; }
        }
    }
}
