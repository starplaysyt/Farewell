using System.Collections.Frozen;
using Farewell.Abstractions.Presentation;

namespace Farewell.Presentation.Internal;

internal sealed class PresentationProvider(
    Dictionary<(Type, PresentationContext), Func<object, string>> presenters)
    : IPresentationProvider
{
    private readonly FrozenDictionary<(Type, PresentationContext), Func<object, string>>
        _presenters = presenters.ToFrozenDictionary();

    public string Present(object? target, PresentationContext context = default)
    {
        if (target is null) return "null";

        var key = (target.GetType(), context);
        if (_presenters.TryGetValue(key, out var fn))
            return fn(target);

        return target.ToString() ?? "null";
    }

    public bool CanPresent(Type type, PresentationContext context = default)
        => _presenters.ContainsKey((type, context));
}