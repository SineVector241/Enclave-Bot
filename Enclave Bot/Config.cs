using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Serilog;

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
                else
                {
                    string botConfigJson = File.ReadAllText(_configFolder + "/" + _configFile);
                    BotConfiguration = JsonConvert.DeserializeObject<BotConfig>(botConfigJson);
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("{0} - {1}", e.InnerException?.Message ?? e.Message, e.StackTrace));
            }
        }
    }

    public struct BotConfig
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
    }
}
