namespace Logging;

public sealed  class ProcessLoggerLazy: ILogger
{
    private static readonly Lazy<ProcessLoggerLazy> _instance = new Lazy<ProcessLoggerLazy> (() => new ProcessLoggerLazy());
    private readonly string _logFilePath;

    private ProcessLoggerLazy()
    {
        var currentDirectory = "C:\\Users\\Ричи\\RiderProjects\\CipherSolution\\Logging";
        var logDirectory = Path.Combine(currentDirectory, "ProcessLog");

        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        _logFilePath = Path.Combine(logDirectory, "ProcessLog.log");
    }
    
    public static ILogger Instance => _instance.Value;
    
    public void LogD(string message, Exception exception)
    {
        var timestamp = DateTime.Now;
        string logMessage = $"[PROCESS] {timestamp:yyyy-MM-dd HH:mm:ss.fff} - Message: {message}";
        File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
    }
}