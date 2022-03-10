using Discord;
using System.Net;
using Discord.WebSocket;
using Discord.Interactions;

namespace Enclave_Bot.Core
{
    public struct UserCooldown
    {
        public ulong UserID { get; set; }
        public int CooldownSeconds { get; set; }
        public DateTime DateTime { get; set; }
    }

    public struct CooldownResponse
    {
        public int Seconds { get; set; }
        public bool CooledDown { get; set; }
    }
    public class Utils
    {
        Random rnd = new Random();
        private static List<UserCooldown> Beg = new List<UserCooldown>();
        private static List<UserCooldown> Deposit = new List<UserCooldown>();
        private static List<UserCooldown> Withdraw = new List<UserCooldown>();
        private static List<UserCooldown> Steal = new List<UserCooldown>();
        private static List<UserCooldown> Stolen = new List<UserCooldown>();
        private static List<UserCooldown> Work = new List<UserCooldown>();
        private static List<UserCooldown> JobHire = new List<UserCooldown>();
        private static List<UserCooldown> Mine = new List<UserCooldown>();
        private static List<UserCooldown> XPCooldown = new List<UserCooldown>();
        private static List<UserCooldown> Suggestion = new List<UserCooldown>();
        public string GetRequest(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            using WebResponse webResponse = request.GetResponse();
            using Stream webStream = webResponse.GetResponseStream();

            using StreamReader reader = new StreamReader(webStream);
            string data = reader.ReadToEnd();
            return data;
        }

        public CooldownResponse Cooldown(SocketUser user, string type, int Seconds = 0)
        {
            var list = new List<UserCooldown>();
            switch(type)
            {
                case "Beg": list = Beg;
                    break;
                case "Deposit": list = Deposit;
                    break;
                case "Withdraw": list = Withdraw;
                    break;
                case "Steal": list = Steal;
                    break;
                case "Stolen": list = Stolen;
                    break;
                case "Work": list = Work;
                    break;
                case "JobHire": list = JobHire;
                    break;
                case "Mine": list = Mine;
                    break;
                case "XP": list = XPCooldown;
                    break;
                case "Suggestion": list = Suggestion;
                    break;
            }
            UserCooldown tempUser = list.FirstOrDefault(x => x.UserID == user.Id);
            if(tempUser.UserID != 0)
            {
                if((DateTime.Now - tempUser.DateTime).TotalSeconds >= tempUser.CooldownSeconds)
                {
                    var value = list.Find(x => x.UserID == tempUser.UserID);
                    value.DateTime = DateTime.Now;
                    list.Remove(tempUser);
                    list.Add(value);
                    return new CooldownResponse() { CooledDown = true, Seconds = Seconds - (int)(DateTime.Now - tempUser.DateTime).TotalSeconds };
                }
                else
                {
                    return new CooldownResponse() { CooledDown = false, Seconds = Seconds - (int)(DateTime.Now - tempUser.DateTime).TotalSeconds };
                }
            }
            else
            {
                UserCooldown NewUser = new UserCooldown();
                NewUser.UserID = user.Id;
                NewUser.CooldownSeconds = Seconds;
                NewUser.DateTime = DateTime.Now;
                list.Add(NewUser);
                return new CooldownResponse() { CooledDown = true, Seconds = Seconds - (int)(DateTime.Now - tempUser.DateTime).TotalSeconds };
            }
        }

        public CooldownResponse CheckCooldown(SocketUser user, string type)
        {
            var list = new List<UserCooldown>();
            switch (type)
            {
                case "Beg": list = Beg;
                    break;
                case "Deposit": list = Deposit;
                    break;
                case "Withdraw": list = Withdraw;
                    break;
                case "Steal": list = Steal;
                    break;
                case "Stolen": list = Stolen;
                    break;
                case "Work": list = Work;
                    break;
                case "JobHire": list = JobHire;
                    break;
                case "Mine": list = Mine;
                    break;
                case "XP": list = XPCooldown;
                    break;
                case "Suggestion": list = Suggestion;
                    break;
            }
            var tempUser = list.FirstOrDefault(x => x.UserID == user.Id);
            if (tempUser.UserID != 0)
            {
                return new CooldownResponse() { Seconds = tempUser.CooldownSeconds - (int)(DateTime.Now - tempUser.DateTime).TotalSeconds, CooledDown = tempUser.CooldownSeconds - (int)(DateTime.Now - tempUser.DateTime).TotalSeconds == 0 ? true: false};
            }
            return new CooldownResponse() { Seconds = 0, CooledDown = true};
        }

        public List<UserCooldown> CheckCooldownList(string type)
        {
            var list = new List<UserCooldown>();
            switch (type)
            {
                case "Beg":
                    list = Beg;
                    break;
                case "Deposit":
                    list = Deposit;
                    break;
                case "Withdraw":
                    list = Withdraw;
                    break;
                case "Steal":
                    list = Steal;
                    break;
                case "Stolen":
                    list = Stolen;
                    break;
                case "Work":
                    list = Work;
                    break;
                case "JobHire":
                    list = JobHire;
                    break;
                case "Mine":
                    list = Mine;
                    break;
                case "XP":
                    list = XPCooldown;
                    break;
                case "Suggestion":
                    list = Suggestion;
                    break;
            }
            return list;
        }

        public Color randomColor()
        {
            Color randomColor = new Color(GenerateRandomInt(rnd), GenerateRandomInt(rnd), GenerateRandomInt(rnd));
            return randomColor;
        }
        public int GenerateRandomInt(Random rnd)
        {
            return rnd.Next(256);
        }
    }
}
