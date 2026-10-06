using Farewell.Abstractions.DI;
using Farewell.Abstractions.Logging;

namespace Farewell.Abstractions.Extensions;

public static class LoggerExtensions
{
    extension(ILogger logger)
    {
        public void LogTrace(string message, string sender = "Default", Exception? exception = null)
            => logger.Log(LogLevel.Trace, message, sender, exception);

        public void LogDebug(string message, string sender = "Default", Exception? exception = null)
        {
            logger.Log(LogLevel.Debug, message, sender, exception);
        }

        public void LogInfo(string message, string sender = "Default", Exception? exception = null)
            => logger.Log(LogLevel.Info, message, sender, exception);

        public void LogWarn(string message, string sender = "Default", Exception? exception = null)
            => logger.Log(LogLevel.Warn, message, sender, exception);

        public void LogError(string message, string sender = "Default", Exception? exception = null)
            => logger.Log(LogLevel.Error, message, sender, exception);

        public void LogFatal(string message, string sender = "Default", Exception? exception = null)
            => logger.Log(LogLevel.Fatal, message, sender, exception);

        public void LogErrorAndThrow(string message, Exception exception)
        {
            logger.LogError(message, "Default", exception);
            throw exception;
        }

        public void LogFatalAndThrow(string message, Exception exception)
        {
            logger.LogFatal(message, "Default", exception);
            throw exception;
        }
    }

    extension(IServiceBuilder builder)
    {
        public IServiceBuilder AddLogger<TLogger>() where TLogger : class, ILogger
            => builder.AddSingleton<ILogger, TLogger>();
        
        public IServiceBuilder AddLogging<TLogger>(object key) where TLogger : class, ILogger
            => builder.AddKeyedSingleton<ILogger, TLogger>(key);
        
        public IServiceBuilder AddLogging(Func<IServiceProvider, ILogger> loggerFactory)
            => builder.AddSingleton(loggerFactory);
    }
}