using WordleGame;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0].ToLower())
            {
                case "standalone":
                    var cheatingGame = new CheatingWordleGame();
                    cheatingGame.Run();
                    break;
                case "server":
                    var cheatingServer = new CheatingWordleServer();
                    await cheatingServer.StartAsync();
                    break;
                case "client":
                    var cheatingClient = new CheatingWordleClient();
                    await cheatingClient.StartAsync();
                    break;
                default:
                    Console.WriteLine("Usage:");
                    Console.WriteLine("  dotnet run -- standalone    # Run standalone cheating game");
                    Console.WriteLine("  dotnet run -- server        # Run cheating server");
                    Console.WriteLine("  dotnet run -- client        # Run cheating client");
                    break;
            }
        }
        else
        {
            // Default to standalone cheating game
            var cheatingGame = new CheatingWordleGame();
            cheatingGame.Run();
        }
    }
} 