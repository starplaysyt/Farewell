namespace Farewell.Abstractions.Validation;

public interface IAsyncValidator<in T>
{
    ValueTask<ValidationReport> ValidateAsync(
        T instance,
        IServiceProvider serviceProvider,
        IValidationCollector collector,
        CancellationToken ct = default);
}