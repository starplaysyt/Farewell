namespace Farewell.Abstractions.Mapping;

public interface IMappingProvider
{
    TResult Map<TSource, TResult>(TSource source);
    TResult Map<TSource, TResult>(TSource source, Func<TSource, TResult> factory);
}