namespace Farewell.Abstractions.Logging;

public interface ILogger
{
    public void Log(LogLevel level, string message, string sender, Exception? exception = null);
}