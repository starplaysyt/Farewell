namespace Farewell.Abstractions.Validation;

public interface IValidationProvider
{
    ValidationReport ValidateAll<T>(T instance, string context = "Default");
    ValidationStatus ValidateBreak<T>(T instance, string context = "Default");
}