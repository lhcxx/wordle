using WordleGame;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0].ToLower())
            {
                case "server":
                    await RunServer(args);
                    break;
                case "client":
                    await RunClient(args);
                    break;
                default:
                    Console.WriteLine("Usage:");
                    Console.WriteLine("  dotnet run -- standalone    # Run standalone game");
                    Console.WriteLine("  dotnet run -- server        # Run server");
                    Console.WriteLine("  dotnet run -- client        # Run client");
                    break;
            }
        }
        else
        {
            // Default to standalone game
            var game = new WordleGame.WordleGame();
            game.Run();
        }
    }

    static async Task RunServer(string[] args)
    {
        Console.WriteLine("Starting Wordle Server...");
        
        var port = 8888;
        if (args.Length > 1 && int.TryParse(args[1], out int customPort))
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

    static async Task RunClient(string[] args)
    {
        Console.WriteLine("Wordle Client");
        Console.WriteLine("=============");
        
        string serverAddress = "localhost";
        int port = 8888;

        if (args.Length > 1)
            serverAddress = args[1];
        if (args.Length > 2 && int.TryParse(args[2], out int customPort))
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