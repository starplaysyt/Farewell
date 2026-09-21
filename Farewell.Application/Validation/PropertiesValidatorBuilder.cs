using System.Linq.Expressions;
using Farewell.Abstractions.Validation;

namespace Farewell.Application.Validation;

public sealed class PropertiesValidatorBuilder<T>
{
    private readonly List<IPropertyRuleBuilder<T>> _builders = new();

    public PropertyRuleBuilder<T, TValue> Rule<TValue>(
        Expression<Func<T, TValue>> selector)
    {
        var compiled = selector.Compile();
        var name = ExtractName(selector);
        var builder = new PropertyRuleBuilder<T, TValue>(name, compiled);
        _builders.Add(builder);
        return builder;
    }

    internal Func<T, ValidationStatus?>[] BuildEvaluators()
        => _builders
            .Select(b => b.BuildEvaluator())
            .ToArray();

    private static string ExtractName<TValue>(Expression<Func<T, TValue>> expr)
        => expr.Body switch
        {
            MemberExpression m => m.Member.Name,
            UnaryExpression { Operand: MemberExpression m } => m.Member.Name,
            _ => throw new ArgumentException("Selector must be a member expression")
        };
}