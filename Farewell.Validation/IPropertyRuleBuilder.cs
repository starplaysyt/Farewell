using Farewell.Abstractions.Validation;

namespace Farewell.Validation;

internal interface IPropertyRuleBuilder<in T>
{
    Func<T, ValidationStatus?> BuildEvaluator();
}