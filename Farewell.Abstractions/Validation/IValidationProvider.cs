namespace Farewell.Abstractions.Validation;

public interface IValidationProvider
{
    ValueTask<ValidationReport> ValidateAsync<T>(
        T instance,
        string context = "Default",
        bool breakOnFirst = false,
        CancellationToken ct = default);

    ValidationReport Validate<T>(
        T instance,
        string context = "Default",
        bool breakOnFirst = false);
}