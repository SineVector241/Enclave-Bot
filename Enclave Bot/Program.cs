namespace Enclave_Bot
{
    class Program
    {
        static void Main(string[] args) =>
            new Bot().MainAsync().GetAwaiter().GetResult();
    }
}