using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Internal;

internal sealed class CompiledValidator<T>
{
    private readonly Func<T, IServiceProvider, CancellationToken, ValueTask<ValidationStatus?>>[] _evaluators;

    internal CompiledValidator(
        Func<T, IServiceProvider, CancellationToken, ValueTask<ValidationStatus?>>[] evaluators)
    {
        _evaluators = evaluators;
    }

    public async ValueTask<ValidationReport> ValidateAsync(
        T instance,
        IServiceProvider sp,
        IValidationCollector collector,
        CancellationToken ct)
    {
        foreach (var evaluator in _evaluators)
        {
            ct.ThrowIfCancellationRequested();

            var status = await evaluator(instance, sp, ct).ConfigureAwait(false);
            if (status is null) continue;

            if (!collector.Collect(status))
                break;
        }

        return collector.ToReport();
    }
}