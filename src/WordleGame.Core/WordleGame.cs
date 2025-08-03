using System;
using System.Collections.Generic;
using System.Linq;

namespace WordleGame
{
    /// <summary>
    /// Core logic class for the Wordle game, responsible for main game flow and rule implementation
    /// including game initialization, round management, guess validation, and result display
    /// </summary>
    public class WordleGame
    {
        /// <summary>
        /// Game configuration object containing word list and game parameters
        /// </summary>
        private readonly GameConfiguration _config;
        
        /// <summary>
        /// Random number generator for selecting answer words
        /// </summary>
        private readonly Random _random;
        
        /// <summary>
        /// Current game's answer word
        /// </summary>
        private string _answer = string.Empty;
        
        /// <summary>
        /// Current round number
        /// </summary>
        private int _currentRound = 0;
        
        /// <summary>
        /// Whether the game has been won
        /// </summary>
        private bool _gameWon = false;

        /// <summary>
        /// Constructor that initializes game configuration and random number generator
        /// </summary>
        public WordleGame()
        {
            _config = new GameConfiguration();
            _random = new Random();
        }

        /// <summary>
        /// Starts the main game loop, displays game rules and handles multiple games
        /// </summary>
        public void Run()
        {
            // Display game welcome message and rule instructions
            Console.WriteLine("Welcome to Wordle!");
            Console.WriteLine("Guess the 5-letter word in 6 attempts.");
            Console.WriteLine("H = Hit (correct letter, correct position)");
            Console.WriteLine("P = Present (correct letter, wrong position)");
            Console.WriteLine("M = Miss (letter not in word)");
            Console.WriteLine();

            // Main game loop supporting multiple games
            while (true)
            {
                StartNewGame();
                PlayGame();
                
                // Ask if player wants to continue
                Console.WriteLine();
                Console.Write("Play again? (y/n): ");
                var playAgain = Console.ReadLine()?.ToLower();
                if (playAgain != "y" && playAgain != "yes")
                    break;
                
                Console.WriteLine();
            }

            Console.WriteLine("Thanks for playing Wordle!");
        }

        /// <summary>
        /// Starts a new game by selecting a random word and resetting game state
        /// </summary>
        private void StartNewGame()
        {
            _answer = _config.WordList[_random.Next(_config.WordList.Count)].ToUpper();
            _currentRound = 0;
            _gameWon = false;
        }

        /// <summary>
        /// Main game loop that handles player input, guess validation, and result display
        /// </summary>
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

        /// <summary>
        /// Validates if the player's guess is valid (5 letters, all alphabetic, in word list)
        /// </summary>
        /// <param name="guess">The player's guess to validate</param>
        /// <returns>True if the guess is valid, false otherwise</returns>
        private bool IsValidGuess(string guess)
        {
            if (string.IsNullOrEmpty(guess) || guess.Length != 5)
                return false;

            if (!guess.All(char.IsLetter))
                return false;

            return _config.WordList.Contains(guess.ToUpper());
        }

        /// <summary>
        /// Evaluates the player's guess against the answer word using Wordle rules
        /// Returns an array indicating Hit (correct letter, correct position),
        /// Present (correct letter, wrong position), or Miss (letter not in word)
        /// </summary>
        /// <param name="guess">The player's guess to evaluate</param>
        /// <returns>Array of LetterResult indicating the result for each position</returns>
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

        /// <summary>
        /// Displays the result of the player's guess with color-coded letters
        /// Green for hits, yellow for present letters, gray for misses
        /// </summary>
        /// <param name="guess">The player's guess</param>
        /// <param name="result">The evaluation result for each position</param>
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

    /// <summary>
    /// Enumeration representing the result of a letter evaluation in Wordle
    /// </summary>
    public enum LetterResult
    {
        /// <summary>
        /// No result assigned yet
        /// </summary>
        None,
        
        /// <summary>
        /// Correct letter in correct position
        /// </summary>
        Hit,
        
        /// <summary>
        /// Correct letter in wrong position
        /// </summary>
        Present,
        
        /// <summary>
        /// Letter not in the word
        /// </summary>
        Miss
    }
} 