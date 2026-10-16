namespace Farewell.Abstractions.Validation;

public interface ICompiledValidator<in T>
{
    ValueTask<ValidationReport> ValidateAsync(
        T instance,
        IServiceProvider sp,
        IValidationCollector collector,
        CancellationToken ct);
}