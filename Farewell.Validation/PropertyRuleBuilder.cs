using Farewell.Abstractions.Validation;
using Farewell.Validation.Internal;

namespace Farewell.Validation;

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

    public PropertyRuleBuilder<T, TValue> Must(IValidationConstraint<TValue> constraint)
    {
        _constraints.Add(new ConstraintEntry<TValue> { Constraint = constraint });
        return this;
    }

    public PropertyRuleBuilder<T, TValue> Must(Func<TValue, bool> check, ValidationCode code)
    {
        _constraints.Add(new ConstraintEntry<TValue>
        {
            InlineCheck = check,
            InlineCode = code
        });
        return this;
    }

    public PropertyRuleBuilder<T, TValue> When(
        Func<T, bool> condition,
        Action<ConstraintGroupBuilder<T, TValue>> group)
    {
        var groupBuilder = new ConstraintGroupBuilder<T, TValue>(_constraints, condition);
        group(groupBuilder);
        return this;
    }

    Func<T, IServiceProvider, CancellationToken, ValueTask<ValidationStatus?>>
        IPropertyRuleBuilder<T>.BuildEvaluator()
    {
        var selector = _selector;
        var propertyName = _propertyName;
        var when = _when;
        var unless = _unless;

        var checks = _constraints
            .Select(entry => BuildConstraintCheck(entry, propertyName))
            .ToArray();

        return async (instance, sp, ct) =>
        {
            if (when != null && !when(instance)) return null;
            if (unless != null && unless(instance)) return null;

            var value = selector(instance);

            foreach (var check in checks)
            {
                var status = await check(instance, value, sp, ct).ConfigureAwait(false);
                if (status is not null) return status;
            }

            return null;
        };
    }

    private static Func<T, TValue, IServiceProvider, CancellationToken, ValueTask<ValidationStatus?>>
        BuildConstraintCheck(ConstraintEntry<TValue> entry, string propertyName)
    {
        var entryWhen = entry.When;

        if (entry.Constraint is not null)
        {
            var constraint = entry.Constraint;
            var code = constraint.Code;

            return async (instance, value, sp, ct) =>
            {
                if (entryWhen != null && !entryWhen(instance)) return null;
                return await constraint.CheckAsync(value, sp, ct).ConfigureAwait(false)
                    ? null
                    : ValidationStatus.Error(code, propertyName);
            };
        }

        var inlineCheck = entry.InlineCheck!;
        var inlineCode = entry.InlineCode!.Value;

        return (instance, value, _, _) =>
        {
            if (entryWhen != null && !entryWhen(instance)) return new ValueTask<ValidationStatus?>((ValidationStatus?)null);
            return new ValueTask<ValidationStatus?>(inlineCheck(value)
                ? null
                : ValidationStatus.Error(inlineCode, propertyName));
        };
    }
}