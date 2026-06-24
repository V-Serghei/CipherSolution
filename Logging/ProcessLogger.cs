namespace Logging;

public sealed class ProcessLogger
{
    private static ProcessLogger? _instance;
    private static readonly object Lock = new();
    private readonly string _logFilePath;

    private ProcessLogger()
    {
        string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "process");
        Directory.CreateDirectory(logDirectory);
        _logFilePath = Path.Combine(logDirectory, "ProcessLog.log");
    }

    public static ProcessLogger Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    _instance ??= new ProcessLogger();
                }
            }
            return _instance;
        }
    }

    public void LogD(string message, string result = "Success")
    {
        string logMessage = $"[LOG] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: {message} - Result: {result}";
        File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
    }
}
