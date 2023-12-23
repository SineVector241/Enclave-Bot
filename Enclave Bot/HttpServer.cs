using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace Enclave_Bot
{
    public class HttpServer
    {
        private readonly HttpListener Listener;
        private System.Timers.Timer CodeExpiry;
        public static Dictionary<ulong, ServerCode> ServerCodes { get; } = new Dictionary<ulong, ServerCode>();

        public HttpServer()
        {
            Listener = new HttpListener();
            CodeExpiry = new System.Timers.Timer(5000);
            CodeExpiry.Elapsed += CheckForExpiredCodes;
        }

        public void Start(ushort Port)
        {
            Listener.Prefixes.Add($"http://*:{Port}/");
            Listener.Start();
            Listener.BeginGetContext(new AsyncCallback(Listen), null);
            CodeExpiry.Start();
        }

        public void Stop()
        {
            Listener.Stop();
            Listener.Close();
            CodeExpiry.Stop();
        }

        private void Listen(IAsyncResult result)
        {
            try
            {
                var ctx = Listener.EndGetContext(result);
                Listener.BeginGetContext(new AsyncCallback(Listen), null);
                if (ctx.Request.HttpMethod == "POST")
                {
                    try
                    {
                        var content = new StreamReader(ctx.Request.InputStream).ReadToEnd();
                        var packet = JObject.Parse(content);
                    }
                    catch
                    {
                        SendResponse(ctx, HttpStatusCode.BadRequest, "Invalid Data!");
                    }
                }
            }
            catch
            {
                Stop();
            }
        }

        public void SendResponse(HttpListenerContext ctx, HttpStatusCode code, string Content)
        {
            var content = Encoding.UTF8.GetBytes(Content);
            ctx.Response.StatusCode = (int)code;
            ctx.Response.ContentType = "text/plain";
            ctx.Response.OutputStream.Write(content, 0, content.Length);
            ctx.Response.OutputStream.Close();
        }

        private static void CheckForExpiredCodes(object? sender, System.Timers.ElapsedEventArgs e)
        {
            var toRemove = ServerCodes.Where(x => DateTime.UtcNow.Subtract(x.Value.CreatedAt).TotalMinutes > 5);
            foreach(var code in toRemove)
            {
                ServerCodes.Remove(code.Key);
            }
        }
    }
}
