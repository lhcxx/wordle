using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WordleGame
{
    public class WordleServer
    {
        private readonly TcpListener _listener;
        private readonly GameConfiguration _config;
        private readonly Dictionary<string, GameSession> _sessions;
        private readonly Random _random;
        private bool _isRunning;

        public WordleServer(int port = 8888)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _config = new GameConfiguration();
            _sessions = new Dictionary<string, GameSession>();
            _random = new Random();
        }

        public async Task StartAsync()
        {
            _listener.Start();
            _isRunning = true;
            Console.WriteLine($"Wordle Server started on port {((IPEndPoint)_listener.LocalEndpoint).Port}");
            Console.WriteLine("Waiting for clients...");

            while (_isRunning)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accepting client: {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            var clientId = Guid.NewGuid().ToString();
                            var remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                Console.WriteLine($"Client {clientId} connected from {remoteEndPoint?.Address ?? IPAddress.Any}");

            try
            {
                using var stream = client.GetStream();
                var buffer = new byte[1024];

                // Send welcome message
                await SendMessageAsync(stream, "Welcome to Wordle Server!");
                await SendMessageAsync(stream, $"You have {_config.MaxRounds} attempts to guess the word.");
                await SendMessageAsync(stream, "Enter your guess (5-letter word):");

                var session = new GameSession
                {
                    ClientId = clientId,
                    Answer = _config.WordList[_random.Next(_config.WordList.Count)].ToUpper(),
                    MaxRounds = _config.MaxRounds,
                    CurrentRound = 0,
                    IsGameWon = false
                };

                _sessions[clientId] = session;

                while (client.Connected && !session.IsGameWon && session.CurrentRound < session.MaxRounds)
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    var response = await ProcessGuessAsync(session, message);
                    await SendMessageAsync(stream, response);

                    if (session.IsGameWon || session.CurrentRound >= session.MaxRounds)
                    {
                        var gameOverMessage = session.IsGameWon 
                            ? $"Congratulations! You won in {session.CurrentRound} rounds!"
                            : $"Game Over! The word was: {session.Answer}";
                        await SendMessageAsync(stream, gameOverMessage);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client {clientId}: {ex.Message}");
            }
            finally
            {
                _sessions.Remove(clientId);
                client.Close();
                Console.WriteLine($"Client {clientId} disconnected");
            }
        }

        private Task<string> ProcessGuessAsync(GameSession session, string guess)
        {
            session.CurrentRound++;

            // Input validation
            if (string.IsNullOrEmpty(guess) || guess.Length != 5)
            {
                session.CurrentRound--; // Don't count invalid guesses
                return Task.FromResult("Invalid guess! Please enter a 5-letter word.");
            }

            if (!guess.All(char.IsLetter))
            {
                session.CurrentRound--;
                return Task.FromResult("Invalid guess! Please enter only letters.");
            }

            var upperGuess = guess.ToUpper();
            if (!_config.WordList.Contains(upperGuess))
            {
                session.CurrentRound--;
                return Task.FromResult("Invalid guess! Word not in dictionary.");
            }

            // Evaluate guess
            var result = EvaluateGuess(upperGuess, session.Answer);
            var resultDisplay = FormatResult(upperGuess, result);

            // Check if won
            if (upperGuess == session.Answer)
            {
                session.IsGameWon = true;
            }

            return Task.FromResult($"Round {session.CurrentRound}/{session.MaxRounds}\n{resultDisplay}\nEnter your guess (5-letter word):");
        }

        private LetterResult[] EvaluateGuess(string guess, string answer)
        {
            var result = new LetterResult[5];
            var answerLetters = answer.ToCharArray();
            var guessLetters = guess.ToCharArray();
            var usedPositions = new bool[5];

            // First pass: find exact matches (Hits)
            for (int i = 0; i < 5; i++)
            {
                if (guessLetters[i] == answerLetters[i])
                {
                    result[i] = LetterResult.Hit;
                    usedPositions[i] = true;
                }
            }

            // Second pass: find letters in wrong position (Present)
            for (int i = 0; i < 5; i++)
            {
                if (result[i] == LetterResult.Hit)
                    continue;

                for (int j = 0; j < 5; j++)
                {
                    if (!usedPositions[j] && guessLetters[i] == answerLetters[j])
                    {
                        result[i] = LetterResult.Present;
                        usedPositions[j] = true;
                        break;
                    }
                }

                if (result[i] == LetterResult.None)
                {
                    result[i] = LetterResult.Miss;
                }
            }

            return result;
        }

        private string FormatResult(string guess, LetterResult[] result)
        {
            var sb = new StringBuilder();
            sb.Append("Result: ");
            
            for (int i = 0; i < 5; i++)
            {
                var symbol = result[i] switch
                {
                    LetterResult.Hit => "🟢",
                    LetterResult.Present => "🟡",
                    LetterResult.Miss => "⚪",
                    _ => "⚪"
                };
                
                sb.Append($"{symbol}{guess[i]}");
                if (i < 4) sb.Append(" "); // Add space between letters but not at the end
            }
            
            return sb.ToString();
        }

        private async Task SendMessageAsync(NetworkStream stream, string message)
        {
            var data = Encoding.UTF8.GetBytes(message + "\n");
            await stream.WriteAsync(data, 0, data.Length);
            await stream.FlushAsync(); // Ensure message is sent immediately
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
        }
    }

    public class GameSession
    {
        public string ClientId { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public int MaxRounds { get; set; } = 6;
        public int CurrentRound { get; set; } = 0;
        public bool IsGameWon { get; set; } = false;
    }
} 