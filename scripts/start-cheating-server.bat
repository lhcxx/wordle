@echo off
echo Starting Cheating Wordle Server...
dotnet run --project src\WordleGame.Cheating\WordleGame.Cheating.csproj -- server
pause 