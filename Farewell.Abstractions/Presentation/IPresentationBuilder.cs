using System.Linq.Expressions;

namespace Farewell.Abstractions.Presentation;

public interface IPresentationBuilder
{
    /// <summary>
    /// Registers presentation of type T in selected context.
    /// template: literal string "{PropName} {$RecursiveProp}" or localized key-resolver.
    /// </summary>
    IPresentationBuilder Add<T>(
        PresentationContext context,
        string template);

    IPresentationBuilder Add<T>(
        PresentationContext context,
        string template,
        params (string name, Expression<Func<T, object?>> value)[] bindings);

    /// <summary>
    /// Registers presentation of an aggregate: header + element collection.
    /// </summary>
    IPresentationBuilder AddAggregate<T, TItem>(
        PresentationContext context,
        string headerTemplate,
        string itemTemplate,
        Expression<Func<T, IEnumerable<TItem>>> source,
        string separator = "");

    /// <summary>
    /// State-driven presentation, selecting presentation by state.
    /// </summary>
    IStatedBuilder<T, TState> AddStated<T, TState>(
        PresentationContext context,
        Expression<Func<T, TState>> selector)
        where TState : notnull;
}

public interface IStatedBuilder<T, in TState> where TState : notnull
{
    IStatedBuilder<T, TState> When(TState state, string template);
    IStatedBuilder<T, TState> Else(string template);
}