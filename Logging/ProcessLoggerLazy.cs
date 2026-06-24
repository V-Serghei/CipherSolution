namespace Logging;

public sealed class ProcessLoggerLazy : ILogger
{
    private static readonly Lazy<ProcessLoggerLazy> _instance = new(() => new ProcessLoggerLazy());
    private readonly string _logFilePath;

    private ProcessLoggerLazy()
    {
        string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "process");
        Directory.CreateDirectory(logDirectory);
        _logFilePath = Path.Combine(logDirectory, "ProcessLog.log");
    }

    public static ILogger Instance => _instance.Value;

    public void LogD(string message, Exception exception)
    {
        string logMessage = $"[PROCESS] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}";
        File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
    }
}
