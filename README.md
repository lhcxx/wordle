# Wordle Console Game

A C# console implementation of the popular Wordle game, featuring the same scoring rules as the original NYTimes Wordle game.

## Features

- **5-letter word guessing game** with the same rules as the original Wordle
- **Configurable word list** loaded from JSON file
- **Configurable maximum rounds** (default: 6 rounds)
- **Color-coded feedback** for each guess:
  - 🟢 Green: Hit (correct letter, correct position)
  - 🟡 Yellow: Present (correct letter, wrong position)  
  - ⚪ Gray: Miss (letter not in word)
- **Input validation** ensuring only valid 5-letter words are accepted
- **Win/Lose detection** based on maximum rounds

## Game Rules

1. The game randomly selects a 5-letter word from the configured word list
2. You have 6 attempts to guess the word
3. After each guess, you receive feedback:
   - **Hit (Green)**: Letter is in the correct position
   - **Present (Yellow)**: Letter is in the word but wrong position
   - **Miss (Gray)**: Letter is not in the word
4. You win if you guess the word within 6 attempts
5. You lose if you fail to guess the word after 6 attempts

## Configuration

### Word List
The game uses a JSON configuration file (`words.json`) containing 100 5-letter words. You can modify this file to add or change words.

### Maximum Rounds
The maximum number of rounds can be configured in the `GameConfiguration.cs` file by changing the `MaxRounds` property (default: 6).

## How to Run

1. **Prerequisites**: .NET 8.0 SDK
2. **Build the project**:
   ```bash
   dotnet build
   ```
3. **Run the game**:
   ```bash
   dotnet run
   ```

## File Structure

- `Program.cs` - Main entry point
- `WordleGame.cs` - Core game logic and scoring
- `GameConfiguration.cs` - Configuration management
- `words.json` - Word list configuration
- `WordleGame.csproj` - Project file

## Example Game Session

```
Welcome to Wordle!
Guess the 5-letter word in 6 attempts.
H = Hit (correct letter, correct position)
P = Present (correct letter, wrong position)
M = Miss (letter not in word)

Round 1/6
Enter your guess: HELLO
Result: H E L L O

Round 2/6
Enter your guess: WORLD
Result: W O R L D

Congratulations! You won in 2 rounds!

Play again? (y/n): n
Thanks for playing Wordle!
```

## Technical Details

- **Scoring Algorithm**: Implements the exact same logic as the original Wordle game
- **Case Insensitive**: All words are converted to uppercase for comparison
- **Input Validation**: Ensures guesses are exactly 5 letters and exist in the word list
- **Error Handling**: Graceful fallback to default words if configuration file is missing or corrupted
- **JSON Configuration**: Uses System.Text.Json for configuration management

## Customization

To add your own words, edit the `words.json` file and add 5-letter words to the "Words" array. The game will automatically load the new word list on startup.