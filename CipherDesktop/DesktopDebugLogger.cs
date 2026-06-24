using System;
using System.IO;

namespace CipherDesktop;

internal static class DesktopDebugLogger
{
    private static readonly object SyncRoot = new();
    private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
    private static readonly string LogFilePath = Path.Combine(LogDirectory, "desktop-debug.log");

    public static string FilePath => LogFilePath;

    public static void Info(string message)
    {
        Write("INFO", message);
    }

    public static void Error(string message, Exception exception)
    {
        Write("ERROR", $"{message} | {exception.GetType().Name}: {exception.Message}");
    }

    private static void Write(string level, string message)
    {
        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}";

        lock (SyncRoot)
        {
            Directory.CreateDirectory(LogDirectory);
            Console.WriteLine(line);
            File.AppendAllText(LogFilePath, line + Environment.NewLine);
        }
    }
}
