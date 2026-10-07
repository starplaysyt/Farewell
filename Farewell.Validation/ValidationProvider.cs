using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Validation;
using Farewell.Validation.Collectors;

namespace Farewell.Validation;

public sealed class ValidationProvider : IValidationProvider
{
    private readonly IServiceProvider _sp;

    public ValidationProvider(IServiceProvider sp) => _sp = sp;

    public ValueTask<ValidationReport> ValidateAsync<T>(
        T instance,
        IValidationCollector collector,
        string context = "Default",
        CancellationToken ct = default)
    {
        var validator = _sp.GetRequiredKeyedService<IAsyncValidator<T>>(context);
        return validator.ValidateAsync(instance, _sp, collector, ct);
    }

    public ValueTask<ValidationReport> ValidateAllAsync<T>(
        T instance, string context = "Default", CancellationToken ct = default)
        => ValidateAsync(instance, new CollectAllCollector(), context, ct);

    public ValueTask<ValidationReport> ValidateFirstAsync<T>(
        T instance, string context = "Default", CancellationToken ct = default)
        => ValidateAsync(instance, new BreakOnFirstCollector(), context, ct);

    public ValidationReport ValidateAll<T>(T instance, string context = "Default")
        => ValidateAllAsync(instance, context)
            .ConfigureAwait(false).GetAwaiter().GetResult();

    public ValidationReport ValidateFirst<T>(T instance, string context = "Default")
        => ValidateFirstAsync(instance, context)
            .ConfigureAwait(false).GetAwaiter().GetResult();
}
