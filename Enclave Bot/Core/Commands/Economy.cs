using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Serilog;

namespace Enclave_Bot.Core.Commands
{
    public class Economy : ModuleBase<SocketCommandContext>
    {
        [Command("balance")]
        [Alias("bal")]
        [Summary("Sends your bank and wallet balance")]
        public async Task Test()
        {
            
        }
    }

    class EconomyConfig
    {
        private const string _economyFolder = "Resources";
        private const string _economyFile = "economy.json";
        private static EconomyFile _economy;

        static EconomyConfig()
        {
            try
            {
                if (!Directory.Exists(_economyFolder))
                {
                    Directory.CreateDirectory(_economyFile);
                }

                if (File.Exists(_economyFolder + "/" + _economyFile))
                {
                    _economy = new EconomyFile();
                    string economyJson = JsonConvert.SerializeObject(_economy, Formatting.Indented);
                    File.Create(_economyFolder + "/" + _economyFile);
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("{0} - {1}", e.InnerException?.Message ?? e.Message, e.StackTrace));
            }
        }

        private struct EconomyFile
        {
            public string ID { get; private set; }
        }
    }
}
