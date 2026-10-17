using System.Collections.Concurrent;
using Farewell.Abstractions.Mapping;
using Farewell.Mapping.Internal;

namespace Farewell.Mapping;

public sealed class MappingProvider : IMappingProvider
{
    public static readonly MappingProvider Instance = new();

    private readonly ConcurrentDictionary<(Type Source, Type Target), Delegate> _cache = new();

    public TResult Map<TSource, TResult>(TSource source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var key = (typeof(TSource), typeof(TResult));
        var mapper = (Func<TSource, TResult>)_cache.GetOrAdd(key, _ => BuildMapper<TSource, TResult>());
        return mapper(source);
    }

    public TResult Map<TSource, TResult>(TSource source, Func<TSource, TResult> factory)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(factory);
        return factory(source);
    }

    private static Func<TSource, TResult> BuildMapper<TSource, TResult>()
    {
        var strategy = StrategyResolver.Resolve(typeof(TResult));

        return strategy switch
        {
            MappingStrategy.Properties => PropertyMappingBuilder.Build<TSource, TResult>(),
            MappingStrategy.Constructor => ConstructorMappingBuilder.Build<TSource, TResult>(),
            _ => throw new InvalidOperationException($"Unknown mapping strategy: {strategy}")
        };
    }
}