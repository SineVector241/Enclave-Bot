using Newtonsoft.Json;

namespace Enclave_Bot
{
    class Config
    {
        private const string _configFolder = "Resources";
        private const string _configFile = "config.json";
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

                string botConfig = File.ReadAllText(_configFolder + "/" + _configFile);
                BotConfiguration = JsonConvert.DeserializeObject<BotConfig>(botConfig);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}]: [ERROR] => An error occured in Config.cs \nError Info:\n{ex}");
            }
        }
    }

    public struct BotConfig
    {
        public string Token { get; set; }
        public ushort HttpServerPort { get; set; }
    }
}
