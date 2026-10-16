using System.Linq.Expressions;
using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Internal;

public sealed class ConstraintBuilder<T> : IConstraintBuilder<T>
{
    private readonly List<Func<T, IServiceProvider, IValidationCollector, CancellationToken,
            ValueTask<bool>>>
        _evaluators = [];

    public IPropertyConstraintBuilder<TValue?> Property<TValue>(
        Expression<Func<T, TValue>> property,
        Func<T, bool>? when = null)
    {
        var propertyName = ExtractPropertyName(property);
        var getter = property.Compile();
        var builder = new PropertyConstraintBuilder<TValue?>();

        _evaluators.Add(async (instance, sp, collector, ct) =>
        {
            if (when is not null && !when(instance))
                return true;

            var value = getter(instance);

            foreach (var check in builder.BuildChecks())
            {
                var passed = await check.Check(value, sp, ct).ConfigureAwait(false);
                if (passed) continue;
                var status = ValidationStatus.Error(check.Code, propertyName);
                if (!collector.Collect(status))
                    return false;
            }

            return true;
        });

        return builder;
    }

    public ICompiledValidator<T> Compile()
        => new CompiledValidator<T>([.. _evaluators]);

    private static string ExtractPropertyName<TValue>(Expression<Func<T, TValue>> expr)
    {
        return expr.Body switch
        {
            MemberExpression member => member.Member.Name,
            UnaryExpression { Operand: MemberExpression unaryMember } => unaryMember.Member.Name,
            _ => throw new ArgumentException($"Expression '{expr}' does not refer to a property.",
                nameof(expr))
        };
    }
}