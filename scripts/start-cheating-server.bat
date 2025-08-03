@echo off
echo Starting Cheating Wordle Server...
cd ..
dotnet run --project src\WordleGame.Cheating\WordleGame.Cheating.csproj -- server
pause 