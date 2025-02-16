namespace Logging;

public sealed class ProcessLogger
{
    private static ProcessLogger _instance;
    private static readonly object Lock = new object();
    private string logFilePath;
    private ProcessLogger() { 
        var currentDirectory = "C:\\Users\\Ричи\\RiderProjects\\CipherSolution\\Logging";
        var logDirectory = Path.Combine(currentDirectory, "DataLog");

        if(!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        logFilePath = Path.Combine(logDirectory, "log.txt");
        
    }
    
    public static ProcessLogger Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ProcessLogger();
                    }
                }
            }

            return _instance;
        }
    }

    public void Log(string message, string result = "Succese")
    {
        var timestamp = DateTime.Now;
        string logMessage = $"[LOG] {timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}: Message: {message} - Result: {result}";

        File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
    }
}