using Farewell.Abstractions.Validation;
using Farewell.Validation.Internal;

namespace Farewell.Validation;

/// <summary>
/// Fluent-builder for one property
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TValue"></typeparam>
public sealed class PropertyRuleBuilder<T, TValue> : IPropertyRuleBuilder<T>
{
    private readonly string _propertyName;
    private readonly Func<T, TValue> _selector;
    private readonly List<ConstraintEntry<TValue>> _constraints = new();
    private Func<T, bool>? _when;
    private Func<T, bool>? _unless;

    internal PropertyRuleBuilder(string propertyName, Func<T, TValue> selector)
    {
        _propertyName = propertyName;
        _selector = selector;
    }

    /// <summary>
    /// Constraints on the whole rule
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public PropertyRuleBuilder<T, TValue> When(Func<T, bool> condition)
    {
        _when = condition;
        return this;
    }

    public PropertyRuleBuilder<T, TValue> Unless(Func<T, bool> condition)
    {
        _unless = condition;
        return this;
    }

    /// <summary>
    /// Constraint without condition
    /// </summary>
    public PropertyRuleBuilder<T, TValue> Must(IValidationConstraint<TValue> constraint)
    {
        _constraints.Add(new ConstraintEntry<TValue> { Constraint = constraint });
        return this;
    }

    /// <summary>
    /// Inline checkup without new class
    /// </summary>
    public PropertyRuleBuilder<T, TValue> Must(Func<TValue, bool> check, ValidationCode code)
    {
        _constraints.Add(new ConstraintEntry<TValue>
        {
            InlineCheck = check,
            InlineCode = code
        });
        return this;
    }

    /// <summary>
    /// Constraint on one group
    /// </summary>
    public PropertyRuleBuilder<T, TValue> When(
        Func<T, bool> condition,
        Action<ConstraintGroupBuilder<T, TValue>> group)
    {
        var groupBuilder = new ConstraintGroupBuilder<T, TValue>(_constraints, condition);
        group(groupBuilder);
        return this;
    }

    Func<T, ValidationStatus?> IPropertyRuleBuilder<T>.BuildEvaluator()
    {
        var selector = _selector;
        var propertyName = _propertyName;
        var when = _when;
        var unless = _unless;

        var checks = _constraints
            .Select(entry => BuildConstraintCheck(entry, propertyName))
            .ToArray();

        return instance =>
        {
            if (when != null && !when(instance)) return null;
            if (unless != null && unless(instance)) return null;

            var value = selector(instance);

            return checks.Select(check => check(instance, value))
                .OfType<ValidationStatus>()
                .FirstOrDefault();
        };
    }

    private static Func<T, TValue, ValidationStatus?> BuildConstraintCheck(
        ConstraintEntry<TValue> entry,
        string propertyName)
    {
        var entryWhen = entry.When;
        
        var constraint = entry.Constraint;
        if (constraint == null)
            throw new ArgumentException("Constraint from entry was null.");
        var code = constraint.Code;

        return (instance, value) =>
        {
            if (entryWhen != null && instance != null && !entryWhen(instance)) return null;
            return constraint.Check(value)
                ? null
                : new ValidationStatus(code, propertyName);
        };
    }
}