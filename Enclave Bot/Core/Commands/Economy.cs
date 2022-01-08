using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

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
        private static EconomyFile Economy;

        static EconomyConfig()
        {
            if (!Directory.Exists(_economyFolder)) Directory.CreateDirectory(_economyFile);
            if (File.Exists(_economyFolder + "/" + _economyFile))
            {
                Economy = new EconomyFile();
                string json = JsonConvert.SerializeObject(Economy, Formatting.Indented);
                File.Create(_economyFolder + "/" + _economyFile);
            }
        }

        private struct EconomyFile
        {
            public string ID { get; private set; }
        }
    }
}
