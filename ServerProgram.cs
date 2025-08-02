using WordleGame;

class ServerProgram
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Wordle Server...");
        
        var server = new WordleServer(8888);
        
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