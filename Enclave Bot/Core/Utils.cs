using Discord;
using System.Net;
using Discord.WebSocket;
using Discord.Interactions;

namespace Enclave_Bot.Core
{
    public class Utils
    {
        Random rnd = new Random();
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

        public Color randomColor()
        {
            Color randomColor = new Color(GenerateRandomInt(rnd), GenerateRandomInt(rnd), GenerateRandomInt(rnd));
            return randomColor;
        }
        private static int GenerateRandomInt(Random rnd)
        {
            return rnd.Next(256);
        }
    }
}
