# Wordle Console Game

A C# console implementation of the popular Wordle game, featuring the same scoring rules as the original NYTimes Wordle game. The project includes both a standalone console version and a client/server architecture.

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

## Configuration

### Word List
The game uses a JSON configuration file (`words.json`) containing 100 5-letter words. You can modify this file to add or change words.

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
   dotnet build
   ```
2. **Run the standalone game**:
   ```bash
   dotnet run --project WordleGame.csproj
   ```
   or explicitly:
   ```bash
   dotnet run --project WordleGame.csproj -- standalone
   ```

### Client/Server Mode

#### Start the Server
1. **Using batch file** (Windows):
   ```bash
   start-server.bat
   ```
2. **Using command line**:
   ```bash
   dotnet run --project WordleGame.csproj -- server
   ```
3. **With custom port**:
   ```bash
   dotnet run --project WordleGame.csproj -- server 9999
   ```

#### Start the Client
1. **Using batch file** (Windows):
   ```bash
   start-client.bat
   ```
2. **Using command line**:
   ```bash
   dotnet run --project WordleGame.csproj -- client
   ```
3. **With custom server address**:
   ```bash
   dotnet run --project WordleGame.csproj -- client 192.168.1.100
   ```
4. **With custom server address and port**:
   ```bash
   dotnet run --project WordleGame.csproj -- client 192.168.1.100 9999
   ```

## File Structure

### Core Files
- `Program.cs` - Main entry point (supports standalone, server, and client modes)
- `WordleGame.cs` - Core game logic and scoring
- `WordleServer.cs` - Server implementation
- `WordleClient.cs` - Client implementation
- `GameConfiguration.cs` - Configuration management
- `words.json` - Word list configuration
- `WordleGame.csproj` - Project file

### Scripts
- `start-server.bat` - Windows batch file to start server
- `start-client.bat` - Windows batch file to start client

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
Result: 🟢H ⚪E ⚪L ⚪L ⚪O

Your guess: WORLD
Round 2/6
Result: ⚪W 🟢O ⚪R ⚪L ⚪D

Congratulations! You won in 2 rounds!

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

## Security Features

### Client/Server Architecture
- **Answer Protection**: Client never receives the answer until game over
- **Server-side Validation**: All game logic and validation on server
- **Session Isolation**: Each client has independent game sessions
- **Input Sanitization**: Server validates all client inputs

## Customization

### Adding Words
To add your own words, edit the `words.json` file and add 5-letter words to the "Words" array. The game will automatically load the new word list on startup.

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