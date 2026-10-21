using System.Linq.Expressions;
using Farewell.Abstractions.Presentation;

namespace Farewell.Presentation.Internal;

internal abstract record Registration(Type TargetType, PresentationContext Context);

internal sealed record SimpleRegistration(
    Type TargetType,
    PresentationContext Context,
    ParsedTemplate Template,
    IReadOnlyDictionary<string, LambdaExpression>?
        CustomBindings // name -> Expression<Func<T, object>>
) : Registration(TargetType, Context);

internal sealed record AggregateRegistration(
    Type TargetType,
    Type ItemType,
    PresentationContext Context,
    ParsedTemplate HeaderTemplate,
    ParsedTemplate ItemTemplate,
    LambdaExpression Source, // Expression<Func<T, IEnumerable<TItem>>>
    string Separator
) : Registration(TargetType, Context);

internal sealed record StatedRegistration(
    Type TargetType,
    Type StateType,
    PresentationContext Context,
    LambdaExpression Selector, // Expression<Func<T, TState>>
    Dictionary<object, ParsedTemplate> Cases,
    ParsedTemplate? Fallback
) : Registration(TargetType, Context);