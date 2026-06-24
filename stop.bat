@echo off
setlocal EnableDelayedExpansion

echo.
echo  ================================================
echo   CipherSolution - Shutdown
echo  ================================================
echo.

set "STOPPED=0"

for /f "tokens=2 delims=," %%p in ('tasklist /FI "IMAGENAME eq dotnet.exe" /FO CSV /NH 2^>nul') do (
    set "PID=%%~p"
    wmic process where "ProcessId=!PID!" get CommandLine 2>nul | findstr /i "CipherDesktop ApplicationL" >nul
    if !errorlevel! equ 0 (
        taskkill /PID !PID! /F >nul 2>&1
        if !errorlevel! equ 0 (
            echo  Stopped dotnet process PID !PID!.
            set "STOPPED=1"
        )
    )
)

if "%STOPPED%"=="0" (
    echo  No running CipherSolution instance found.
)

echo.
echo  Done.
timeout /t 2 >nul
