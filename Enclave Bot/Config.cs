using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Enclave_Bot
{
    class Config
    {
        private const string _configFolder = "Resources";
        private const string _configFile = "config.json";
        private const string _activityFile = "useractivity.json";
        public static BotConfig BotConfiguration;

        static Config()
        {
            try
            {
                if (!Directory.Exists(_configFolder))
                {
                    Directory.CreateDirectory(_configFolder);
                }

                if (!File.Exists(_configFolder + "/" + _configFile))
                {
                    BotConfiguration = new BotConfig();
                    string botConfigJson = JsonConvert.SerializeObject(BotConfiguration, Formatting.Indented);
                    File.WriteAllText(_configFolder + "/" + _configFile, botConfigJson);
                }
                if (!File.Exists(_configFolder + "/" + _activityFile))
                {
                    File.WriteAllText(_configFolder + "/" + _activityFile, "{}");
                }
                string botConfig = File.ReadAllText(_configFolder + "/" + _configFile);
                BotConfiguration = JsonConvert.DeserializeObject<BotConfig>(botConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\u001b[97m[{DateTime.Now}]: [\u001b[31mERROR\u001b[97m] => An error occured in Config.cs \nError Info:\n{ex}");
            }
        }
        public JObject GetUserActivities()
        {
            var users = JObject.Parse(File.ReadAllText("Resources/useractivity.json"));
            return users;
        }

        public void WriteToActivities(JObject obj)
        {
            File.WriteAllText("Resources/useractivity.json", obj.ToString());
        }
    }

    public struct BotConfig
    {
        public string Token { get; set; }
    }
}
