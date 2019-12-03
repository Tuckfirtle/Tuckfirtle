@echo off

SETLOCAL 

SET project=Tuckfirtle.sln
SET framework=netcoreapp3.0

SET configuration_linux=Release_linux
SET configuration_osx=Release_osx
SET configuration_win=Release_win

SET platform_arm=arm
SET platform_x64=x64
SET platform_x86=x86

SET output=./bin/Release

dotnet publish %project% /p:Platform=%platform_x86% -c %configuration_win% -f %framework% -o %output%/net-core-win-x86 -r win-x86 --self-contained

ENDLOCAL