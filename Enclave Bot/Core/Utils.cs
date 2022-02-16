using Discord;
using System.Net;
using Discord.WebSocket;
using Discord.Interactions;

namespace Enclave_Bot.Core
{
    public struct UserCooldown
    {
        public string CooldownType { get; set; }
        public ulong UserID { get; set; }
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
        private static List<UserCooldown> users = new List<UserCooldown>();
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

        public CooldownResponse Cooldown(UserCooldown user, int Seconds)
        {
            var tempUser = users.FirstOrDefault(x => x.UserID == user.UserID && x.CooldownType == user.CooldownType);
            if(tempUser.UserID != 0)
            {
                if((DateTime.Now - tempUser.DateTime).TotalSeconds >= Seconds)
                {
                    var value = users.Find(x => x.UserID == tempUser.UserID && x.CooldownType == tempUser.CooldownType);
                    value.DateTime = DateTime.Now;
                    users.Remove(tempUser);
                    users.Add(value);
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
                NewUser.UserID = user.UserID;
                NewUser.CooldownType = user.CooldownType;
                NewUser.DateTime = DateTime.Now;
                users.Add(NewUser);
                return new CooldownResponse() { CooledDown = true, Seconds = Seconds - (int)(DateTime.Now - tempUser.DateTime).TotalSeconds };
            }
        }

        public CooldownResponse CheckCooldown(SocketUser user, string cooldownType, int Seconds)
        {
            var tempUser = users.FirstOrDefault(x => x.UserID == user.Id && x.CooldownType == cooldownType);
            if (tempUser.UserID != 0)
            {
                return new CooldownResponse() { Seconds = Seconds - (int)(DateTime.Now - tempUser.DateTime).TotalSeconds, CooledDown = Seconds - (int)(DateTime.Now - tempUser.DateTime).TotalSeconds == 0 ? true: false};
            }
            return new CooldownResponse() { Seconds = 0, CooledDown = true};
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
