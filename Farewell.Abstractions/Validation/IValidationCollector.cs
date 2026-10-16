namespace Farewell.Abstractions.Validation;

public interface IValidationCollector
{
    bool Collect(ValidationStatus status);
    ValidationReport ToReport();
}