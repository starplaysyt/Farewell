using Microsoft.Extensions.Logging;

namespace Farewell.Compatibility.MS.Logging.Backward;

// This one makes Farewell work with ILogger from MS
public class FarewellBackwardLogger(ILoggerFactory loggerFactory) : Abstractions.Logging.ILogger
{
    private readonly ILogger _logger = loggerFactory.CreateLogger("Farewell.ILogger");
    public void Log(Abstractions.Logging.LogLevel level, string message, string sender, Exception? exception = null)
    {
        if (!_logger.IsEnabled((LogLevel)level)) return;
        
        _logger.Log((LogLevel)level, "[{DateTime}] [ {Sender} ] {Message}", DateTime.Now, sender, message);
    }
}