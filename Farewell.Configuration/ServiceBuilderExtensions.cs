using Farewell.Abstractions.Configuration;
using Farewell.Abstractions.Configuration.Exceptions;
using Farewell.Abstractions.DI;
using Microsoft.Extensions.DependencyInjection;

namespace Farewell.Configuration;

public static class ServiceBuilderExtensions
{
    /// <summary>
    /// Registers a new resolver for a specific extension
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="fileExtension"></param>
    /// <typeparam name="TResolver"></typeparam>
    /// <returns></returns>
    public static IServiceBuilder AddConfigResolver<TResolver>(
        this IServiceBuilder builder,
        string fileExtension)
        where TResolver : class, IConfigResolver
    {
        builder.AddKeyedSingleton<IConfigResolver, TResolver>(fileExtension);
        return builder;
    }

    /// <summary>
    /// Registers a JSON resolver
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IServiceBuilder AddJsonConfigResolver(this IServiceBuilder builder)
    {
        builder.AddConfigResolver<JsonConfigResolver>(".json");
        return builder;
    }

    /// <summary>
    /// Registers a configuration object
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="absolutePath"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ConfigurationResolverNotFoundException"></exception>
    public static IServiceBuilder AddConfig<T>(
        this IServiceBuilder builder,
        string absolutePath)
        where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(absolutePath))
            throw new ArgumentException(
                "Configuration path cannot be null or empty.", nameof(absolutePath));

        var extension = Path.GetExtension(absolutePath);

        if (string.IsNullOrEmpty(extension))
            throw new ArgumentException(
                $"Cannot determine file extension from path '{absolutePath}'.",
                nameof(absolutePath));

        builder.AddSingleton<IConfigurationProvider<T>>(sp =>
        {
            var resolver = sp.GetKeyedService<IConfigResolver>(extension);

            return resolver is null
                ? throw new ConfigurationResolverNotFoundException(extension)
                : new ConfigurationProvider<T>(absolutePath, resolver);
        });

        return builder;
    }
    
    public static IServiceBuilder AddConfig<T>(
        this IServiceBuilder builder,
        string relativePath,
        bool relativeToBaseDirectory)
        where T : class, new()
    {
        var absolutePath = relativeToBaseDirectory
            ? Path.Combine(AppContext.BaseDirectory, relativePath)
            : relativePath;

        return builder.AddConfig<T>(absolutePath);
    }
}