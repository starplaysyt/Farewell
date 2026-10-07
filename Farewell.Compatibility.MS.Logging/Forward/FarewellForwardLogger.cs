using Microsoft.Extensions.Logging;

namespace Farewell.Compatibility.MS.Logging.Forward;

/// <summary>
/// WARNING: USING THAT CAN AND WILL CAUSE PROBLEMS. THIS DEFINITELY WILL BE OBSOLETE IN NEW VERSIONS 
/// </summary>
/// <param name="logger"></param>
public class FarewellForwardLogger(Abstractions.Logging.ILogger logger) : ILogger
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        logger.Log((Abstractions.Logging.LogLevel)logLevel, formatter(state, exception), eventId.Name ?? "Default");
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}