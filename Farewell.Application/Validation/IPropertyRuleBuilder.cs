using Farewell.Abstractions.Validation;

namespace Farewell.Application.Validation;

internal interface IPropertyRuleBuilder<in T>
{
    Func<T, ValidationStatus?> BuildEvaluator();
}