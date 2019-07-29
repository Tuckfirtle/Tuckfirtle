@echo off

SETLOCAL 

SET project=src/Tuckfirtle.Core.Benchmark.Builder.csproj
SET configuration=Release
SET framework=net472

dotnet run --project %project% -c %configuration% --framework %framework%

ENDLOCAL

pause