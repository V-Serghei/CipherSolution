namespace Logging;

public interface ILogger
{
    public void LogD(string message, Exception exception);
    
}