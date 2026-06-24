@echo off
setlocal EnableDelayedExpansion
title CipherSolution - Setup

set "RUN_CONSOLE=0"
set "RUN_TESTS=0"

for %%a in (%*) do (
    if /I "%%~a"=="--console" set "RUN_CONSOLE=1"
    if /I "%%~a"=="--test" set "RUN_TESTS=1"
)

echo.
echo  ================================================
echo   CipherSolution - Setup and Launch
echo  ================================================
echo.

echo [1/4] Checking prerequisites...

dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo.
    echo  [ERROR] .NET SDK not found.
    echo  Install .NET 10 SDK from: https://dotnet.microsoft.com/download
    echo.
    pause
    exit /b 1
)

for /f "tokens=*" %%v in ('dotnet --version 2^>nul') do set "DOTNET_VER=%%v"
echo        .NET SDK: %DOTNET_VER%

echo %DOTNET_VER% | findstr /b "10\." >nul
if %errorlevel% neq 0 (
    echo  [WARN] Expected .NET 10.x. Current version: %DOTNET_VER%
    echo         Project targets net10.0 and net10.0-windows.
)

echo.
echo [2/4] Restoring NuGet packages...
dotnet restore CipherSolution.sln --verbosity quiet
if %errorlevel% neq 0 (
    echo  [ERROR] NuGet restore failed.
    pause
    exit /b 1
)
echo        Packages restored.

echo.
echo [3/4] Building solution (Release)...
dotnet build CipherSolution.sln --configuration Release --no-restore --verbosity quiet
if %errorlevel% neq 0 (
    echo  [ERROR] Build failed. See errors above.
    pause
    exit /b 1
)
echo        Build succeeded.

echo.
echo [4/4] Test step...
if "%RUN_TESTS%"=="1" (
    dotnet test CipherSolution.sln --configuration Release --no-build --verbosity quiet
    if !errorlevel! neq 0 (
        echo  [WARN] One or more tests failed. Application will still launch.
    ) else (
        echo        All tests passed.
    )
) else (
    echo        Skipped. Use --test to run tests during startup.
)

echo.
echo  ================================================
echo   Setup complete. Launching application...
echo  ================================================
echo.

if "%RUN_CONSOLE%"=="1" (
    title CipherSolution Console
    dotnet run --project ApplicationL\ApplicationL.csproj --configuration Release --no-build
) else (
    title CipherSolution Desktop Host
    dotnet run --project CipherDesktop\CipherDesktop.csproj --configuration Release --no-build
)

echo.
echo  Application exited.
pause
