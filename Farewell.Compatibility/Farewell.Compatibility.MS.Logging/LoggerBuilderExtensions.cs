using Farewell.Compatibility.MS.Logging.Backward;
using Farewell.Compatibility.MS.Logging.Forward;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ILogger = Farewell.Abstractions.Logging.ILogger;

namespace Farewell.Compatibility.MS.Logging;

public static class LoggerBuilderExtensions
{
    /// <summary>
    /// Adds forward compatibility with ILogger from Microsoft.Extensions.Logging.<br/>
    /// <b>ILogger from Microsoft.Extensions.Logging will use ILogger from Farewell.</b>
    /// Requires calling builder.Logging.ClearProviders(); and adding Farewell logger separately.
    /// </summary>
    /// <remarks>
    /// Using that is currently pretty unsafe and incomplete because of the incompleteness of current Farewell ILogger implementation.
    /// That layer IS NOT cover about 50% of usage scenarios, so use with caution - some logs can be cut or look dumb.
    /// </remarks>
    /// <param name="builder">The ILoggingBuilder</param>
    /// <returns></returns>
    public static ILoggingBuilder AddForwardFarewellLogger(this ILoggingBuilder builder)
    {
        builder.Services.TryAddSingleton<ILoggerProvider, FarewellForwardLoggerProvider>();
        // ServiceDescriptor.Singleton<ILoggerProvider, FarewellForwardLoggerProvider>()
        return builder;
    }

    /// <summary>
    /// Adds backward compatibility with ILogger from Microsoft.Extensions.Logging.<br/>
    /// <b>ILogger from Farewell will use ILogger from Microsoft.Extensions.Logging.</b>
    /// </summary>
    /// <remarks>
    /// Using that is currently pretty unsafe and incomplete because of the differences in current Farewell ILogger implementation
    /// and Microsoft.Extensions.Logging ILogger.
    /// </remarks>
    /// <param name="builder">The ILoggingBuilder</param>
    /// <returns></returns>
    public static ILoggingBuilder AddBackwardFarewellLogger(this ILoggingBuilder builder)
    {
        builder.Services.TryAddSingleton<ILogger, FarewellBackwardLogger>();
        return builder;
    }
}