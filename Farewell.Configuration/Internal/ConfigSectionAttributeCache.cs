using System.Collections.Concurrent;
using System.Reflection;
using Farewell.Abstractions.Attributes.Configuration;
using Farewell.Configuration.Exceptions;

namespace Farewell.Configuration.Internal;

internal static class ConfigSectionAttributeCache
{
    private static readonly ConcurrentDictionary<Type, string> Cache = new();

    public static string GetPath<T>() where T : class
    {
        return Cache.GetOrAdd(typeof(T), static type =>
        {
            var attribute = type.GetCustomAttribute<ConfigSectionAttribute>();

            return attribute is null
                ? throw new ConfigSectionAttributeMissingException(type)
                : attribute.Path;
        });
    }
}