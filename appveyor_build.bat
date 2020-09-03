@echo off

SETLOCAL 

SET project=Tuckfirtle.sln
SET framework=netcoreapp5.0

SET configuration_linux=Release_linux
SET configuration_osx=Release_osx
SET configuration_win=Release_win

SET platform_arm=arm
SET platform_x64=x64
SET platform_x86=x86

SET output=./bin/Release

dotnet publish %project% /p:Platform=%platform_arm% -c %configuration_linux% -f %framework% -o %output%/netcoreapp-linux-arm -r linux-arm --self-contained
dotnet publish %project% /p:Platform=%platform_x64% -c %configuration_linux% -f %framework% -o %output%/netcoreapp-linux-x64 -r linux-x64 --self-contained
dotnet publish %project% /p:Platform=%platform_x64% -c %configuration_osx% -f %framework% -o %output%/netcoreapp-osx-x64 -r osx-x64 --self-contained
dotnet publish %project% /p:Platform=%platform_arm% -c %configuration_win% -f %framework% -o %output%/netcoreapp-win-arm -r win-arm --self-contained
dotnet publish %project% /p:Platform=%platform_x64% -c %configuration_win% -f %framework% -o %output%/netcoreapp-win-x64 -r win-x64 --self-contained
dotnet publish %project% /p:Platform=%platform_x86% -c %configuration_win% -f %framework% -o %output%/netcoreapp-win-x86 -r win-x86 --self-contained

7z a %output%/netcoreapp-linux-arm.tar %output%/netcoreapp-linux-arm
7z a %output%/netcoreapp-linux-arm.tar.gz %output%/netcoreapp-linux-arm.tar -mx=9

7z a %output%/netcoreapp-linux-x64.tar %output%/netcoreapp-linux-x64
7z a %output%/netcoreapp-linux-x64.tar.gz %output%/netcoreapp-linux-x64.tar -mx=9

7z a %output%/netcoreapp-osx-x64.tar %output%/netcoreapp-osx-x64
7z a %output%/netcoreapp-osx-x64.tar.gz %output%/netcoreapp-osx-x64.tar -mx=9

7z a %output%/netcoreapp-win-arm.zip %output%/netcoreapp-win-arm -mx=9
7z a %output%/netcoreapp-win-x64.zip %output%/netcoreapp-win-x64 -mx=9
7z a %output%/netcoreapp-win-x86.zip %output%/netcoreapp-win-x86 -mx=9

ENDLOCAL