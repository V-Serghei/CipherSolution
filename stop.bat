@echo off
echo.
echo  ================================================
echo   CipherSolution - Shutdown
echo  ================================================
echo.

:: Kill the console window running the app (matched by title set in start.bat)
taskkill /FI "WINDOWTITLE eq CipherSolution" /F >nul 2>&1
if %errorlevel% equ 0 (
    echo  Application stopped.
) else (
    echo  No running CipherSolution instance found.
)

:: Also kill any dotnet processes running ApplicationL (safety net)
for /f "tokens=2" %%p in ('tasklist /FI "IMAGENAME eq dotnet.exe" /FO CSV /NH 2^>nul') do (
    wmic process where "ProcessId=%%~p" get CommandLine 2>nul | findstr /i "ApplicationL" >nul
    if !errorlevel! equ 0 (
        taskkill /PID %%~p /F >nul 2>&1
        echo  Killed dotnet process PID %%~p
    )
)

echo.
echo  Done.
timeout /t 2 >nul
