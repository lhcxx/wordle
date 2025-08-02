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
                var messageBuffer = new StringBuilder();

                // Read welcome messages
                while (client.Connected)
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    var receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    messageBuffer.Append(receivedData);

                    // Process complete messages (separated by newlines)
                    var messages = messageBuffer.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    messageBuffer.Clear();

                    foreach (var message in messages)
                    {
                        var trimmedMessage = message.Trim();
                        if (string.IsNullOrEmpty(trimmedMessage)) continue;

                        // Display message with color if it's a result
                        if (trimmedMessage.Contains("Result:"))
                        {
                            DisplayColoredResult(trimmedMessage);
                        }
                        else
                        {
                            Console.WriteLine(trimmedMessage);
                        }

                        // Check if game is over
                        if (trimmedMessage.Contains("Congratulations") || trimmedMessage.Contains("Game Over"))
                        {
                            Console.WriteLine("\nGame ended. Press any key to exit...");
                            Console.ReadKey();
                            return;
                        }

                        // If we received a prompt for input, error message, or round information, get user input
                        if (trimmedMessage.Contains("Enter your guess") || 
                            trimmedMessage.Contains("Invalid guess"))
                        {
                            Console.Write("Your guess: ");
                            var guess = Console.ReadLine()?.Trim();
                            
                            if (!string.IsNullOrEmpty(guess))
                            {
                                var data = Encoding.UTF8.GetBytes(guess + "\n");
                                await stream.WriteAsync(data, 0, data.Length);
                                await stream.FlushAsync(); // Ensure message is sent immediately
                            }
                        }
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

        private void DisplayColoredResult(string resultMessage)
        {
            Console.Write("Result: ");
            
            // Parse the result message to extract the colored parts
            var parts = resultMessage.Replace("Result: ", "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var part in parts)
            {
                // Handle emoji characters properly
                if (part.Length >= 3 && part.StartsWith("🟢"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(part.Substring(2)); // Skip the emoji
                    Console.ResetColor();
                    Console.Write(" ");
                }
                else if (part.Length >= 3 && part.StartsWith("🟡"))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(part.Substring(2)); // Skip the emoji
                    Console.ResetColor();
                    Console.Write(" ");
                }
                else if (part.Length >= 3 && part.StartsWith("⚪"))
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(part.Substring(2)); // Skip the emoji
                    Console.ResetColor();
                    Console.Write(" ");
                }
                else
                {
                    Console.Write(part + " ");
                }
            }
            Console.WriteLine();
        }
    }
} 