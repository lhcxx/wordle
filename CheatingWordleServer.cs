using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace WordleGame
{
    public class CheatingWordleServer
    {
        private readonly TcpListener _listener;
        private readonly GameConfiguration _config;
        private readonly Dictionary<string, CheatingGameSession> _sessions;
        private bool _isRunning;

        public CheatingWordleServer(int port = 8889)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _config = new GameConfiguration();
            _sessions = new Dictionary<string, CheatingGameSession>();
        }

        public async Task StartAsync()
        {
            _listener.Start();
            _isRunning = true;
            Console.WriteLine($"Cheating Wordle Server started on port {((IPEndPoint)_listener.LocalEndpoint).Port}");
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
                await SendMessageAsync(stream, "Welcome to Cheating Wordle Server!");
                await SendMessageAsync(stream, $"You have {_config.MaxRounds} attempts to guess the word.");
                await SendMessageAsync(stream, "Enter your guess (5-letter word):");

                var session = new CheatingGameSession
                {
                    ClientId = clientId,
                    Candidates = new List<string>(_config.WordList),
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
                            : $"Game Over! The word was: {session.CurrentAnswer}";
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

        private Task<string> ProcessGuessAsync(CheatingGameSession session, string guess)
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

            // Find the best (lowest scoring) result
            var result = FindBestResult(session, upperGuess);
            var resultDisplay = FormatResult(upperGuess, result);

            // Check if won
            if (result.All(r => r == LetterResult.Hit))
            {
                session.IsGameWon = true;
            }

            // If this was the last round and we didn't win, don't prompt for another guess
            if (session.CurrentRound >= session.MaxRounds && !session.IsGameWon)
            {
                return Task.FromResult($"Round {session.CurrentRound}/{session.MaxRounds}\n{resultDisplay}");
            }
            
            return Task.FromResult($"Round {session.CurrentRound}/{session.MaxRounds}\n{resultDisplay}\nEnter your guess (5-letter word):");
        }

        private LetterResult[] FindBestResult(CheatingGameSession session, string guess)
        {
            // Calculate scores for all candidates against this guess
            var candidateScores = new List<(string word, int score, LetterResult[] result)>();

            foreach (var candidate in session.Candidates)
            {
                var result = EvaluateGuess(guess, candidate);
                var score = CalculateScore(result);
                candidateScores.Add((candidate, score, result));
            }

            // Find the minimum score
            var minScore = candidateScores.Min(cs => cs.score);

            // Get all candidates with minimum score
            var bestCandidates = candidateScores.Where(cs => cs.score == minScore).ToList();

            // Randomly select one of the best candidates
            var random = new Random();
            var selectedCandidate = bestCandidates[random.Next(bestCandidates.Count)];

            // Update current answer and candidates
            session.CurrentAnswer = selectedCandidate.word;
            UpdateCandidates(session, guess, selectedCandidate.result);

            return selectedCandidate.result;
        }

        private int CalculateScore(LetterResult[] result)
        {
            int score = 0;
            foreach (var letterResult in result)
            {
                switch (letterResult)
                {
                    case LetterResult.Hit:
                        score += 10; // Higher weight for hits
                        break;
                    case LetterResult.Present:
                        score += 1; // Lower weight for presents
                        break;
                    case LetterResult.Miss:
                        score += 0; // No points for misses
                        break;
                }
            }
            return score;
        }

        private void UpdateCandidates(CheatingGameSession session, string guess, LetterResult[] result)
        {
            var newCandidates = new List<string>();

            foreach (var candidate in session.Candidates)
            {
                var candidateResult = EvaluateGuess(guess, candidate);
                
                // Check if this candidate matches the result we chose
                bool matches = true;
                for (int i = 0; i < 5; i++)
                {
                    if (candidateResult[i] != result[i])
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                {
                    newCandidates.Add(candidate);
                }
            }

            session.Candidates.Clear();
            session.Candidates.AddRange(newCandidates);
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
                var status = result[i] switch
                {
                    LetterResult.Hit => "H",
                    LetterResult.Present => "P",
                    LetterResult.Miss => "M",
                    _ => "M"
                };
                
                sb.Append($"{status}{guess[i]}");
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

    public class CheatingGameSession
    {
        public string ClientId { get; set; } = string.Empty;
        public List<string> Candidates { get; set; } = new List<string>();
        public string CurrentAnswer { get; set; } = string.Empty;
        public int MaxRounds { get; set; } = 6;
        public int CurrentRound { get; set; } = 0;
        public bool IsGameWon { get; set; } = false;
    }
} 