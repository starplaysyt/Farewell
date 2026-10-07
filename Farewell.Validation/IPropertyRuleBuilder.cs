using Farewell.Abstractions.Validation;

namespace Farewell.Validation;

internal interface IPropertyRuleBuilder<in T>
{
    Func<T, IServiceProvider, CancellationToken, ValueTask<ValidationStatus?>> BuildEvaluator();
}