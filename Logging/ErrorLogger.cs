namespace Logging
{
    public sealed class ErrorLogger : ILogger
    {
        private static readonly Lazy<ErrorLogger> _instance = new(() => new ErrorLogger());
        private readonly string _logFilePath;

        private ErrorLogger()
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "error");
            Directory.CreateDirectory(logDirectory);
            _logFilePath = Path.Combine(logDirectory, "ErrorLog.log");
        }

        public static ILogger Instance => _instance.Value;

        public void LogD(string message, Exception exception)
        {
            string logMessage =
                $"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message} " +
                $"- {exception.GetType()}: {exception.Message}\nStackTrace: {exception.StackTrace}";
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }
    }
}
