@echo off
setlocal EnableDelayedExpansion
title CipherSolution - Setup

echo.
echo  ================================================
echo   CipherSolution - Setup ^& Launch
echo  ================================================
echo.

:: ─── 1. Check prerequisites ──────────────────────────────────────────────────
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

for /f "tokens=*" %%v in ('dotnet --version 2^>nul') do set DOTNET_VER=%%v
echo        .NET SDK: %DOTNET_VER%

:: Warn if not .NET 10+ (but do not block — allows previews)
echo %DOTNET_VER% | findstr /b "10\." >nul
if %errorlevel% neq 0 (
    echo  [WARN] Expected .NET 10.x. Current version: %DOTNET_VER%
    echo         Project targets net10.0 — build may fail on older SDKs.
)

:: ─── 2. Restore NuGet packages ───────────────────────────────────────────────
echo.
echo [2/4] Restoring NuGet packages...
dotnet restore CipherSolution.sln --verbosity quiet
if %errorlevel% neq 0 (
    echo  [ERROR] NuGet restore failed. Check your internet connection.
    pause
    exit /b 1
)
echo        Packages restored.

:: ─── 3. Build solution ───────────────────────────────────────────────────────
echo.
echo [3/4] Building solution (Release)...
dotnet build CipherSolution.sln --configuration Release --no-restore --verbosity quiet
if %errorlevel% neq 0 (
    echo  [ERROR] Build failed. See errors above.
    pause
    exit /b 1
)
echo        Build succeeded.

:: ─── 4. Run tests ────────────────────────────────────────────────────────────
echo.
echo [4/4] Running tests...
dotnet test CipherSolution.sln --configuration Release --no-build --verbosity quiet
if %errorlevel% neq 0 (
    echo  [WARN] One or more tests failed — check output above.
    echo         Application will still launch.
) else (
    echo        All tests passed.
)

:: ─── Launch ──────────────────────────────────────────────────────────────────
echo.
echo  ================================================
echo   Setup complete. Launching application...
echo   Close this window or press Ctrl+C to stop.
echo  ================================================
echo.

title CipherSolution
dotnet run --project ApplicationL\ApplicationL.csproj --configuration Release --no-build

echo.
echo  Application exited.
pause
