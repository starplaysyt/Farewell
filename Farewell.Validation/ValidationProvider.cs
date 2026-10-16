using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Validation;

namespace Farewell.Validation;

public sealed class ValidationProvider(IServiceProvider serviceProvider) : IValidationProvider
{
    public async ValueTask<ValidationReport> ValidateAsync<T>(
        T instance,
        string context = "Default",
        bool breakOnFirst = false,
        CancellationToken ct = default)
    {
        var validator = serviceProvider.GetRequiredKeyedService<IAsyncValidator<T>>(context);
        IValidationCollector collector = breakOnFirst
            ? new BreakOnFirstCollector()
            : new CollectAllCollector();

        return await validator.ValidateAsync(instance, serviceProvider, collector, ct)
            .ConfigureAwait(false);
    }

    public ValidationReport Validate<T>(
        T instance,
        string context = "Default",
        bool breakOnFirst = false)
        => ValidateAsync(instance, context, breakOnFirst).AsTask()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
}