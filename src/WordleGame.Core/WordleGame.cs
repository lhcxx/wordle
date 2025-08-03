using System;
using System.Collections.Generic;
using System.Linq;

namespace WordleGame
{
    public class WordleGame
    {
        private readonly GameConfiguration _config;
        private readonly Random _random;
        private string _answer = string.Empty;
        private int _currentRound = 0;
        private bool _gameWon = false;

        public WordleGame()
        {
            _config = new GameConfiguration();
            _random = new Random();
        }

        public void Run()
        {
            Console.WriteLine("Welcome to Wordle!");
            Console.WriteLine("Guess the 5-letter word in 6 attempts.");
            Console.WriteLine("H = Hit (correct letter, correct position)");
            Console.WriteLine("P = Present (correct letter, wrong position)");
            Console.WriteLine("M = Miss (letter not in word)");
            Console.WriteLine();

            while (true)
            {
                StartNewGame();
                PlayGame();
                
                Console.WriteLine();
                Console.Write("Play again? (y/n): ");
                var playAgain = Console.ReadLine()?.ToLower();
                if (playAgain != "y" && playAgain != "yes")
                    break;
                
                Console.WriteLine();
            }

            Console.WriteLine("Thanks for playing Wordle!");
        }

        private void StartNewGame()
        {
            _answer = _config.WordList[_random.Next(_config.WordList.Count)].ToUpper();
            _currentRound = 0;
            _gameWon = false;
        }

        private void PlayGame()
        {
            while (_currentRound < _config.MaxRounds && !_gameWon)
            {
                _currentRound++;
                Console.WriteLine($"Round {_currentRound}/{_config.MaxRounds}");
                Console.Write("Enter your guess: ");
                
                var guess = Console.ReadLine()?.ToUpper();
                
                if (string.IsNullOrEmpty(guess) || !IsValidGuess(guess))
                {
                    Console.WriteLine("Invalid guess! Please enter a 5-letter word.");
                    _currentRound--;
                    continue;
                }

                var result = EvaluateGuess(guess);
                DisplayResult(guess, result);
                
                if (guess == _answer)
                {
                    _gameWon = true;
                    Console.WriteLine($"Congratulations! You won in {_currentRound} rounds!");
                }
            }

            if (!_gameWon)
            {
                Console.WriteLine($"Game Over! The word was: {_answer}");
            }
        }

        private bool IsValidGuess(string guess)
        {
            if (string.IsNullOrEmpty(guess) || guess.Length != 5)
                return false;

            if (!guess.All(char.IsLetter))
                return false;

            return _config.WordList.Contains(guess.ToUpper());
        }

        private LetterResult[] EvaluateGuess(string guess)
        {
            var result = new LetterResult[5];
            var answerLetters = _answer.ToCharArray();
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
                var color = result[i] switch
                {
                    LetterResult.Hit => ConsoleColor.Green,
                    LetterResult.Present => ConsoleColor.Yellow,
                    LetterResult.Miss => ConsoleColor.Gray,
                    _ => ConsoleColor.White
                };

                Console.ForegroundColor = color;
                Console.Write(guess[i]);
                Console.ResetColor();
                Console.Write(" ");
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }

    public enum LetterResult
    {
        None,
        Hit,
        Present,
        Miss
    }
} 