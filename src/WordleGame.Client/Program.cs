using WordleGame;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Wordle Client");
        Console.WriteLine("=============");
        
        string serverAddress = "localhost";
        int port = 8888;

        if (args.Length > 0)
            serverAddress = args[0];
        if (args.Length > 1 && int.TryParse(args[1], out int customPort))
            port = customPort;

        var client = new WordleClient(serverAddress, port);
        
        try
        {
            await client.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Client error: {ex.Message}");
        }
    }
} 