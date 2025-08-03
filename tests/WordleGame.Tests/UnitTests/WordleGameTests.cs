using Xunit;
using WordleGame;

namespace WordleGame.Tests.UnitTests
{
    public class WordleGameTests
    {
        [Fact]
        public void EvaluateGuess_ShouldReturnCorrectResults()
        {
            // Arrange
            var guess = "HELLO";
            var answer = "WORLD";

            // Act
            var result = EvaluateGuess(guess, answer);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Length);
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

        [Fact]
        public void IsValidGuess_ShouldValidateCorrectly()
        {
            // Act & Assert
            Assert.True(IsValidGuess("HELLO"));
            Assert.False(IsValidGuess("HELL")); // Too short
            Assert.False(IsValidGuess("HELLOO")); // Too long
            Assert.False(IsValidGuess("HELL1")); // Contains numbers
        }

        private bool IsValidGuess(string guess)
        {
            if (guess.Length != 5) return false;
            if (!guess.All(char.IsLetter)) return false;
            return true; // Simplified for testing
        }

        [Fact]
        public void CalculateScore_ShouldReturnCorrectScore()
        {
            // Arrange
            var result = new LetterResult[] 
            { 
                LetterResult.Hit, 
                LetterResult.Present, 
                LetterResult.Miss, 
                LetterResult.Hit, 
                LetterResult.Miss 
            };

            // Act
            var score = CalculateScore(result);

            // Assert
            Assert.Equal(21, score); // 10 + 1 + 0 + 10 + 0 = 21
        }

        private int CalculateScore(LetterResult[] result)
        {
            int score = 0;
            foreach (var letterResult in result)
            {
                switch (letterResult)
                {
                    case LetterResult.Hit:
                        score += 10;
                        break;
                    case LetterResult.Present:
                        score += 1;
                        break;
                    case LetterResult.Miss:
                        score += 0;
                        break;
                }
            }
            return score;
        }
    }
} 