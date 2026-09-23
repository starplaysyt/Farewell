using Farewell.Abstractions.Configuration;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Exceptions;
using Farewell.Abstractions.Extensions;

namespace Farewell.Configuration;

public static class ConfigurationProviderExtensions
{
    /// <summary>
    /// Registers named IConfigurationProvider in DI container
    /// </summary>
    public static IServiceBuilder AddConfigurationProvider(
        this IServiceBuilder services,
        string key,
        IConfigParser parser,
        bool watchFile = false)
    {
        services.AddKeyedSingleton<IConfigurationProvider>(
            key,
            (_) => new ConfigurationProvider(parser, watchFile)
        );
        
        return services;
    }

    /// <summary>
    /// Registers configuration class in DI container
    /// </summary>
    public static IServiceBuilder AddConfigurationSection<T>(
        this IServiceBuilder services,
        string providerKey)
        where T : class
    {
        services.AddSingleton<T>(sp =>
        {
            var keyServiceProvider =
                sp as IKeyedServiceProvider ??
                throw new UnsupportedOperationException(
                    "Given service provider do not support keyed services. " +
                    "Try to inherit your custom provider from IKeyedServiceProvider.");
            
            var provider = keyServiceProvider
                .GetRequiredKeyedService<IConfigurationProvider>(providerKey);
            return provider.GetSection<T>() 
                   ?? throw new InvalidOperationException(
                       $"Could not resolve configuration section '{typeof(T).Name}' " +
                       $"from provider '{providerKey}'"
                   );
        });

        return services;
    }
}