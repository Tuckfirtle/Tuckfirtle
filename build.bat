@echo off

SETLOCAL 

SET project=Tuckfirtle.sln
SET framework=netcoreapp3.0
SET configuration=Release_win
SET platform=x64
SET output=./bin/Release

dotnet publish %project% /p:Platform=%platform% -c %configuration% -f %framework% -o %output%/net-core-win-x64 -r win-x64 --self-contained

ENDLOCAL

pause