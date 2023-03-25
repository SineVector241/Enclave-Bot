using Discord;
using System.Net;

namespace Enclave_Bot.Core
{
    public static class Utils
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

        public static ModalBuilder UpdateTextInput(this ModalBuilder modal, string customId, Action<TextInputBuilder> input)
        {
            var components = modal.Components.ActionRows.SelectMany(r => r.Components).OfType<TextInputComponent>();
            var component = components.First(c => c.CustomId == customId);

            var builder = new TextInputBuilder
            {
                CustomId = customId,
                Label = component.Label,
                MaxLength = component.MaxLength,
                MinLength = component.MinLength,
                Placeholder = component.Placeholder,
                Required = component.Required,
                Style = component.Style,
                Value = component.Value
            };

            input(builder);

            foreach (var row in modal.Components.ActionRows.Where(row => row.Components.Any(c => c.CustomId == customId)))
            {
                row.Components.RemoveAll(c => c.CustomId == customId);
                row.AddComponent(builder.Build());
            }

            return modal;
        }
    }
}
