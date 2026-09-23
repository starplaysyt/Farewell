using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Internal;

/// <summary>
/// Single constraint description with optional condition
/// </summary>
/// <typeparam name="TValue"></typeparam>
internal sealed class ConstraintEntry<TValue>
{
    public IValidationConstraint<TValue>? Constraint { get; init; }
    public Func<TValue, bool>? InlineCheck { get; init; }
    public ValidationCode? InlineCode { get; init; }
    public Func<object, bool>? When { get; init; }
}