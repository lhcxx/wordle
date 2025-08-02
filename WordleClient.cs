using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WordleGame
{
    public class WordleClient
    {
        private readonly string _serverAddress;
        private readonly int _port;

        public WordleClient(string serverAddress = "localhost", int port = 8888)
        {
            _serverAddress = serverAddress;
            _port = port;
        }

        public async Task StartAsync()
        {
            try
            {
                using var client = new TcpClient();
                Console.WriteLine($"Connecting to server at {_serverAddress}:{_port}...");
                
                await client.ConnectAsync(_serverAddress, _port);
                Console.WriteLine("Connected to server!");

                using var stream = client.GetStream();
                var buffer = new byte[1024];

                // Read welcome messages
                while (client.Connected)
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Console.WriteLine(message);

                    // If we received a prompt for input, get user input
                    if (message.Contains("Enter your guess") || message.Contains("Round"))
                    {
                        Console.Write("Your guess: ");
                        var guess = Console.ReadLine()?.Trim();
                        
                        if (!string.IsNullOrEmpty(guess))
                        {
                            var data = Encoding.UTF8.GetBytes(guess + "\n");
                            await stream.WriteAsync(data, 0, data.Length);
                        }
                    }

                    // Check if game is over
                    if (message.Contains("Congratulations") || message.Contains("Game Over"))
                    {
                        Console.WriteLine("\nGame ended. Press any key to exit...");
                        Console.ReadKey();
                        break;
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
                Console.WriteLine("Make sure the server is running and the address/port are correct.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
} 