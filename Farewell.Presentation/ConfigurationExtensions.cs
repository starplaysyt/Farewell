using Farewell.Abstractions.DI;
using Farewell.Abstractions.Presentation;

namespace Farewell.Presentation;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddServicesOfType<T>(
        this IConfigurationBuilder configurationBuilder, bool useLog = false) where T : class
    {
        var type = typeof(T);
        if (!(type.IsInterface || type.IsAbstract))
            throw new InvalidOperationException(
                "AddServicesOfType<T> supports only interfaces and abstract classes");

        return configurationBuilder;
    }
}