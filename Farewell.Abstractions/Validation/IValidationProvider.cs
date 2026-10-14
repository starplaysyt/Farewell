namespace Farewell.Abstractions.Validation;

public interface IValidationProvider
{
    ValueTask<ValidationReport> ValidateAsync<T>(
        T instance,
        IValidationCollector collector,
        string context = "Default",
        CancellationToken ct = default);

    ValueTask<ValidationReport> ValidateAllAsync<T>(
        T instance,
        string context = "Default",
        CancellationToken ct = default);

    ValueTask<ValidationReport> ValidateFirstAsync<T>(
        T instance,
        string context = "Default",
        CancellationToken ct = default);
}