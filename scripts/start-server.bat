@echo off
echo Starting Wordle Server...
cd ..
dotnet run --project src\WordleGame.Server\WordleGame.Server.csproj
pause 