using Discord;
using System.Net;

namespace Enclave_Bot.Core
{
    public class Utils
    {
        public static string HTTPGetRequest(string url)
        {
            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            using WebResponse webResponse = request.GetResponse();
            using Stream webStream = webResponse.GetResponseStream();

            using StreamReader reader = new StreamReader(webStream);
            string data = reader.ReadToEnd();
            return data;
        }

        public static Color RandomColor()
        {
            Color randomColor = new Color(GenerateRandomInt(), GenerateRandomInt(), GenerateRandomInt());
            return randomColor;
        }
        public static int GenerateRandomInt()
        {
            Random rnd = new Random();
            return rnd.Next(256);
        }

        public static Int32 ToUnixTimestamp(DateTime datetime) { return (int)datetime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds; }
    }
}
