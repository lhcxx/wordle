using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordleGame
{
    public class CheatingWordleGame
    {
        private readonly GameConfiguration _config;
        private readonly List<string> _candidates;
        private string _currentAnswer = string.Empty;
        private int _currentRound;
        private readonly int _maxRounds;
        private bool _isGameWon;

        public CheatingWordleGame()
        {
            _config = new GameConfiguration();
            _candidates = new List<string>(_config.WordList);
            _currentRound = 0;
            _maxRounds = _config.MaxRounds;
            _isGameWon = false;
        }

        public void Run()
        {
            Console.WriteLine("Welcome to Cheating Wordle!");
            Console.WriteLine("Guess the 5-letter word in 6 attempts.");
            Console.WriteLine("H = Hit (correct letter, correct position)");
            Console.WriteLine("P = Present (correct letter, wrong position)");
            Console.WriteLine("M = Miss (letter not in word)");
            Console.WriteLine();

            while (_currentRound < _maxRounds && !_isGameWon)
            {
                _currentRound++;
                Console.WriteLine($"Round {_currentRound}/{_maxRounds}");
                Console.Write("Enter your guess: ");
                
                var guess = Console.ReadLine()?.Trim().ToUpper();
                
                if (string.IsNullOrEmpty(guess) || !IsValidGuess(guess))
                {
                    Console.WriteLine("Invalid guess! Please enter a 5-letter word.");
                    _currentRound--; // Don't count invalid guesses
                    continue;
                }

                // Find the best (lowest scoring) candidate that matches the guess result
                var result = FindBestResult(guess);
                DisplayResult(guess, result);

                // Check if won
                if (result.All(r => r == LetterResult.Hit))
                {
                    _isGameWon = true;
                    Console.WriteLine($"Congratulations! You won in {_currentRound} rounds!");
                }
                else if (_currentRound >= _maxRounds)
                {
                    Console.WriteLine($"Game Over! The word was: {_currentAnswer}");
                }

                Console.WriteLine();
            }

            Console.WriteLine("Thanks for playing Cheating Wordle!");
        }

        private LetterResult[] FindBestResult(string guess)
        {
            // Calculate scores for all candidates against this guess
            var candidateScores = new List<(string word, int score, LetterResult[] result)>();

            foreach (var candidate in _candidates)
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
            _currentAnswer = selectedCandidate.word;
            UpdateCandidates(guess, selectedCandidate.result);

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

        private void UpdateCandidates(string guess, LetterResult[] result)
        {
            var newCandidates = new List<string>();

            foreach (var candidate in _candidates)
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

            _candidates.Clear();
            _candidates.AddRange(newCandidates);
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

        private void DisplayResult(string guess, LetterResult[] result)
        {
            Console.Write("Result: ");
            
            for (int i = 0; i < 5; i++)
            {
                switch (result[i])
                {
                    case LetterResult.Hit:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(guess[i]);
                        Console.ResetColor();
                        break;
                    case LetterResult.Present:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(guess[i]);
                        Console.ResetColor();
                        break;
                    case LetterResult.Miss:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(guess[i]);
                        Console.ResetColor();
                        break;
                }
                Console.Write(" ");
            }
            Console.WriteLine();
        }

        private bool IsValidGuess(string guess)
        {
            if (guess.Length != 5) return false;
            if (!guess.All(char.IsLetter)) return false;
            return _config.WordList.Contains(guess);
        }
    }
} 