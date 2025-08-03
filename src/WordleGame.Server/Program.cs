using WordleGame;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Wordle Server...");
        
        var port = 8888;
        if (args.Length > 0 && int.TryParse(args[0], out int customPort))
            port = customPort;

        var server = new WordleServer(port);
        
        // Handle graceful shutdown
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            Console.WriteLine("\nShutting down server...");
            server.Stop();
        };

        try
        {
            await server.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Server error: {ex.Message}");
        }
    }
} 