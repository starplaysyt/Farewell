using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Internal;

internal sealed class CompiledValidator<T>(
    Func<T, IServiceProvider, IValidationCollector, CancellationToken, ValueTask<bool>>[]
        evaluators)
    : ICompiledValidator<T>
{
    public async ValueTask<ValidationReport> ValidateAsync(
        T instance,
        IServiceProvider sp,
        IValidationCollector collector,
        CancellationToken ct)
    {
        foreach (var evaluator in evaluators)
        {
            var shouldContinue = await evaluator(instance, sp, collector, ct)
                .ConfigureAwait(false);
            if (!shouldContinue)
                return collector.ToReport();
        }

        return collector.ToReport();
    }
}