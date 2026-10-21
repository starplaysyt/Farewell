using System.Linq.Expressions;
using Farewell.Abstractions.Presentation;

namespace Farewell.Presentation.Internal;

internal sealed class PresentationBuilder : IPresentationBuilder
{
    private readonly Dictionary<(Type, PresentationContext), Registration> _registrations = new();

    internal IReadOnlyDictionary<(Type, PresentationContext), Registration> Registrations =>
        _registrations;

    public IPresentationBuilder Add<T>(PresentationContext context, string template)
        => Add<T>(context, template, []);

    public IPresentationBuilder Add<T>(
        PresentationContext context,
        string template,
        params (string name, Expression<Func<T, object?>> value)[] bindings)
    {
        var parsed = TemplateParser.Parse(template);

        Dictionary<string, LambdaExpression>? customBindings = null;
        if (bindings.Length > 0)
        {
            customBindings = new Dictionary<string, LambdaExpression>(bindings.Length);
            foreach (var (name, expr) in bindings)
                customBindings[name] = expr;
        }

        _registrations[(typeof(T), context)] = new SimpleRegistration(
            typeof(T), context, parsed, customBindings);

        return this;
    }

    public IPresentationBuilder AddAggregate<T, TItem>(
        PresentationContext context,
        string headerTemplate,
        string itemTemplate,
        Expression<Func<T, IEnumerable<TItem>>> source,
        string separator = "")
    {
        _registrations[(typeof(T), context)] = new AggregateRegistration(
            typeof(T), typeof(TItem), context,
            TemplateParser.Parse(headerTemplate),
            TemplateParser.Parse(itemTemplate),
            source, separator);

        return this;
    }

    public IStatedBuilder<T, TState> AddStated<T, TState>(
        PresentationContext context,
        Expression<Func<T, TState>> selector)
        where TState : notnull
    {
        var sb = new StatedBuilder<T, TState>(context, selector);
        sb.OnCommit = (reg) => _registrations[(typeof(T), context)] = reg;
        return sb;
    }
}

internal sealed class StatedBuilder<T, TState> : IStatedBuilder<T, TState>
    where TState : notnull
{
    private readonly PresentationContext _context;
    private readonly LambdaExpression _selector;
    private readonly Dictionary<object, ParsedTemplate> _cases = new();
    private ParsedTemplate? _fallback;

    internal Action<StatedRegistration>? OnCommit;

    public StatedBuilder(PresentationContext context, Expression<Func<T, TState>> selector)
    {
        _context = context;
        _selector = selector;
    }

    public IStatedBuilder<T, TState> When(TState state, string template)
    {
        _cases[state] = TemplateParser.Parse(template);
        Commit();
        return this;
    }

    public IStatedBuilder<T, TState> Else(string template)
    {
        _fallback = TemplateParser.Parse(template);
        Commit();
        return this;
    }

    private void Commit()
    {
        OnCommit?.Invoke(new StatedRegistration(
            typeof(T), typeof(TState), _context, _selector,
            new Dictionary<object, ParsedTemplate>(_cases), _fallback));
    }
}