# Wordle Console Game

A C# console implementation of the popular Wordle game, featuring the same scoring rules as the original NYTimes Wordle game. The project includes both a standalone console version, a client/server architecture, and a "host cheating" mode similar to Absurdle.

## Features

### Standalone Version
- **5-letter word guessing game** with the same rules as the original Wordle
- **Configurable word list** loaded from JSON file
- **Configurable maximum rounds** (default: 6 rounds)
- **Color-coded feedback** for each guess:
  - 🟢 Green: Hit (correct letter, correct position)
  - 🟡 Yellow: Present (correct letter, wrong position)  
  - ⚪ Gray: Miss (letter not in word)
- **Input validation** ensuring only valid 5-letter words are accepted
- **Win/Lose detection** based on maximum rounds

### Client/Server Version
- **TCP-based client/server architecture**
- **Multiple concurrent clients** supported
- **Server-side validation** and game logic
- **Client-side user interface** for input/output
- **Network communication** over TCP sockets
- **Session management** for each client

### Cheating Wordle Version (Task 3)
- **Host cheating mode** similar to Absurdle
- **Dynamic candidate management** - host doesn't pre-select answer
- **Lowest score selection** - always gives worst possible result
- **Score-based ranking**: Hit (10 points) > Present (1 point) > Miss (0 points)
- **Candidate filtering** - maintains list of words matching all previous results
- **External observer transparency** - impossible to detect cheating from guesses

## Game Rules

1. The game randomly selects a 5-letter word from the configured word list
2. You have 6 attempts to guess the word
3. After each guess, you receive feedback:
   - **Hit (Green)**: Letter is in the correct position
   - **Present (Yellow)**: Letter is in the word but wrong position
   - **Miss (Gray)**: Letter is not in the word
4. You win if you guess the word within 6 attempts
5. You lose if you fail to guess the word after 6 attempts

## Architecture

### Standalone Mode
- Single console application
- All game logic runs locally
- Direct user interaction

### Client/Server Mode
- **Server**: Handles game logic, validation, and session management
- **Client**: Provides user interface and network communication
- **Network Protocol**: TCP-based communication
- **Security**: Server validates all inputs, client never knows the answer

### Cheating Wordle Mode
- **Standalone**: Single console application with cheating logic
- **Server/Client**: Network-based cheating with dynamic candidate management
- **Algorithm**: Always selects the candidate with lowest score for each guess
- **Transparency**: External observers cannot detect cheating from game behavior

## Configuration

### Word List
The game uses a JSON configuration file (`config/words.json`) containing 100 5-letter words. You can modify this file to add or change words.

### Maximum Rounds
The maximum number of rounds can be configured in the `GameConfiguration.cs` file by changing the `MaxRounds` property (default: 6).

### Network Settings
- **Default Port**: 8888
- **Default Server**: localhost
- **Protocol**: TCP

## How to Run

### Prerequisites
- .NET 8.0 SDK

### Standalone Mode
1. **Build the project**:
   ```bash
   dotnet build wordle.sln
   ```
2. **Run the standalone game**:
   ```bash
   dotnet run --project src/WordleGame.Core/WordleGame.Core.csproj
   ```

### Client/Server Mode

#### Start the Server
```bash
dotnet run --project src/WordleGame.Server/WordleGame.Server.csproj
```

#### Start the Client
```bash
dotnet run --project src/WordleGame.Client/WordleGame.Client.csproj
```

### Cheating Wordle Mode

#### Standalone Cheating Game
```bash
dotnet run --project src/WordleGame.Cheating/WordleGame.Cheating.csproj
```

#### Start the Cheating Server
```bash
dotnet run --project src/WordleGame.Cheating/WordleGame.Cheating.csproj -- server
```

#### Start the Cheating Client
```bash
dotnet run --project src/WordleGame.Cheating/WordleGame.Cheating.csproj -- client
```

## File Structure

### Project Organization
```
wordle/
├── src/
│   ├── WordleGame.Core/           # Core game logic and configuration
│   │   ├── GameConfiguration.cs   # Configuration management
│   │   └── WordleGame.cs         # Core game logic and scoring
│   ├── WordleGame.Client/         # Client implementation
│   │   └── WordleClient.cs       # Client implementation
│   ├── WordleGame.Server/         # Server implementation
│   │   └── WordleServer.cs       # Server implementation
│   └── WordleGame.Cheating/       # Cheating game implementation
│       ├── CheatingWordleGame.cs  # Cheating game logic
│       └── CheatingWordleServer.cs # Cheating server implementation
├── tests/
│   └── WordleGame.Tests/          # Unit and integration tests
│       ├── UnitTests/
│       └── IntegrationTests/
├── config/
│   └── words.json                 # Word list configuration
├── wordle.sln                     # Solution file
└── README.md                      # This file
```

### Core Files
- `src/WordleGame.Core/GameConfiguration.cs` - Configuration management
- `src/WordleGame.Core/WordleGame.cs` - Core game logic and scoring
- `src/WordleGame.Server/WordleServer.cs` - Server implementation
- `src/WordleGame.Client/WordleClient.cs` - Client implementation
- `src/WordleGame.Cheating/CheatingWordleGame.cs` - Cheating game logic
- `src/WordleGame.Cheating/CheatingWordleServer.cs` - Cheating server implementation
- `config/words.json` - Word list configuration

### Test Files
- `tests/WordleGame.Tests/UnitTests/` - Unit tests for core functionality
- `tests/WordleGame.Tests/IntegrationTests/` - Integration tests for client/server communication

## Example Sessions

### Standalone Mode
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

### Client/Server Mode

#### Server Output
```
Starting Wordle Server...
Wordle Server started on port 8888
Waiting for clients...
Client 12345-67890 connected from 127.0.0.1
Client 12345-67890 disconnected
```

#### Client Output
```
Wordle Client
=============
Connecting to server at localhost:8888...
Connected to server!
Welcome to Wordle Server!
You have 6 attempts to guess the word.
Enter your guess (5-letter word):
Your guess: HELLO
Round 1/6
Result: H E L L O  (with colored letters: Green for Hit, Yellow for Present, Gray for Miss)

Your guess: WORLD
Round 2/6
Result: W O R L D  (with colored letters: Green for Hit, Yellow for Present, Gray for Miss)

Congratulations! You won in 2 rounds!

Game ended. Press any key to exit...
```

### Cheating Wordle Mode

#### Standalone Cheating Game
```
Welcome to Cheating Wordle!
Guess the 5-letter word in 6 attempts.
H = Hit (correct letter, correct position)
P = Present (correct letter, wrong position)
M = Miss (letter not in word)

Round 1/6
Enter your guess: HELLO
Result: M E M M M

Round 2/6
Enter your guess: WORLD
Result: M M M M M

Round 3/6
Enter your guess: QUICK
Result: M M M M M

Round 4/6
Enter your guess: JUMPY
Result: M M M M M

Round 5/6
Enter your guess: ZEBRA
Result: M M M M M

Round 6/6
Enter your guess: VIVID
Result: M M M M M

Game Over! The word was: XXXXX
Thanks for playing Cheating Wordle!
```

#### Cheating Server Output
```
Cheating Wordle Server started on port 8889
Waiting for clients...
Client 12345-67890 connected from 127.0.0.1
Client 12345-67890 disconnected
```

#### Cheating Client Output
```
Connecting to cheating server at localhost:8889...
Connected to cheating server!
Welcome to Cheating Wordle Server!
You have 6 attempts to guess the word.
Enter your guess (5-letter word):
Your guess: HELLO
Round 1/6
Result: M E M M M  (with colored letters: Gray for Miss, Yellow for Present, Green for Hit)

Your guess: WORLD
Round 2/6
Result: M M M M M  (all Gray for Miss)

Game Over! The word was: XXXXX

Game ended. Press any key to exit...
```

## Technical Details

### Standalone Mode
- **Scoring Algorithm**: Implements the exact same logic as the original Wordle game
- **Case Insensitive**: All words are converted to uppercase for comparison
- **Input Validation**: Ensures guesses are exactly 5 letters and exist in the word list
- **Error Handling**: Graceful fallback to default words if configuration file is missing or corrupted
- **JSON Configuration**: Uses System.Text.Json for configuration management

### Client/Server Mode
- **Network Protocol**: TCP sockets for reliable communication
- **Session Management**: Each client gets a unique session with its own game state
- **Server-side Validation**: All input validation happens on the server
- **Concurrent Clients**: Server can handle multiple clients simultaneously
- **Graceful Shutdown**: Server handles Ctrl+C for clean shutdown
- **Error Handling**: Robust error handling for network issues

### Cheating Wordle Mode
- **Dynamic Candidate Management**: Host maintains list of possible words
- **Score-based Selection**: Always chooses candidate with lowest score
- **Algorithm**: Hit (10 points) > Present (1 point) > Miss (0 points)
- **Candidate Filtering**: Updates candidate list based on all previous results
- **Transparency**: External observers cannot detect cheating behavior
- **Random Selection**: When multiple candidates have same score, randomly selects one

## Security Features

### Client/Server Architecture
- **Answer Protection**: Client never receives the answer until game over
- **Server-side Validation**: All game logic and validation on server
- **Session Isolation**: Each client has independent game sessions
- **Input Sanitization**: Server validates all client inputs

## Customization

### Adding Words
To add your own words, edit the `config/words.json` file and add 5-letter words to the "Words" array. The game will automatically load the new word list on startup.

### Network Configuration
- **Change Port**: Modify the port number in `ServerProgram.cs` and `ClientProgram.cs`
- **Change Server Address**: Modify the server address in `ClientProgram.cs`
- **Firewall**: Ensure the server port is open in your firewall

## Troubleshooting

### Common Issues
1. **Connection Refused**: Make sure the server is running before starting the client
2. **Port Already in Use**: Change the port number in the server configuration
3. **Firewall Issues**: Allow the application through your firewall
4. **Network Issues**: Check network connectivity between client and server