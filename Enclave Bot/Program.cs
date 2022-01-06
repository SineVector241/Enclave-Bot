// See https://aka.ms/new-console-template for more information
using Discord;
using Discord.WebSocket;

public class EnclaveBot
{
    private DiscordSocketClient _client;
    public static Task Main(string[] args) => new EnclaveBot().MainAsync();

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();

        var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
        _client = new DiscordSocketClient(_config);

        _client.Log += LogAsync;
        _client.MessageReceived += Messaged;
        _client.Ready += () =>
        {
            Console.WriteLine("Bot is connected!");
            return Task.CompletedTask;
        };

        //  You can assign your bot token to a string, and pass that in to connect.
        //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
        var token = "ODA0MjUyODI2OTc2MjU2MDIx.YBJo0A.ZZ7WKrxzym-gJk3ZIOOS9Hjrt78";

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private Task LogAsync(LogMessage message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }

    private async Task Messaged(SocketMessage msg)
    {
        var message = msg as SocketUserMessage;
        if (message == null) return;
        Console.WriteLine(message);
    }
}