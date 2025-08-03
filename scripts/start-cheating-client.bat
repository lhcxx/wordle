@echo off
echo Starting Cheating Wordle Client...
cd ..
dotnet run --project src\WordleGame.Cheating\WordleGame.Cheating.csproj -- client
pause 