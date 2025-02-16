namespace Logging
{
    public sealed class ErrorLogger
    {
        private static readonly Lazy<ErrorLogger> _instance = new Lazy<ErrorLogger>(() => new ErrorLogger());
        private readonly string _logFilePath;

        private ErrorLogger()
        {
            var currentDirectory = "C:\\Users\\Ричи\\RiderProjects\\CipherSolution\\Logging";
            var logDirectory = Path.Combine(currentDirectory, "ErrorLog");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            _logFilePath = Path.Combine(logDirectory, "error_log.txt");
        }

        public static ErrorLogger Instance => _instance.Value;

        public void LogError(string message, Exception exception)
        {
            var timestamp = DateTime.Now;
            string logMessage = $"[ERROR] {timestamp:yyyy-MM-dd HH:mm:ss.fff} - Message: {message} - Exception: {exception.GetType()} - {exception.Message}\nStackTrace: {exception.StackTrace}\n";
            
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }
    }
}