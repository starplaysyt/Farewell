using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Constraints;

internal sealed class ConstraintEntry<TValue>
{
    // For typed constraint
    public IValidationConstraint<TValue>? Constraint { get; init; }

    // For inlined func
    public Func<TValue, bool>? InlineCheck { get; init; }
    public ValidationCode? InlineCode { get; init; }

    // For constraint-level condition (from grouped when)
    public Func<object, bool>? When { get; init; }
}