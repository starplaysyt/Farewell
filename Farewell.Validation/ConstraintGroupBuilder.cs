using Farewell.Abstractions.Validation;
using Farewell.Validation.Internal;

namespace Farewell.Validation;

/// <summary>
/// Builder for group of constraints with one condition.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TValue"></typeparam>
public sealed class ConstraintGroupBuilder<T, TValue>
{
    private readonly List<ConstraintEntry<TValue>> _entries;
    private readonly Func<T, bool> _when;

    internal ConstraintGroupBuilder(List<ConstraintEntry<TValue>> entries, Func<T, bool> when)
    {
        _entries = entries;
        _when = when;
    }

    public ConstraintGroupBuilder<T, TValue> Must(IValidationConstraint<TValue> constraint)
    {
        _entries.Add(new ConstraintEntry<TValue>
        {
            Constraint = constraint,
            When = instance => _when((T)instance)
        });
        return this;
    }

    public ConstraintGroupBuilder<T, TValue> Must(Func<TValue, bool> check, ValidationCode code)
    {
        _entries.Add(new ConstraintEntry<TValue>
        {
            InlineCheck = check,
            InlineCode = code,
            When = instance => _when((T)instance)
        });
        return this;
    }
}