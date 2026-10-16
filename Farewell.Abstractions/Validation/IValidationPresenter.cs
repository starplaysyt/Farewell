namespace Farewell.Abstractions.Validation;

public interface IValidationPresenter
{
    string Format(ValidationStatus status);
    IReadOnlyList<string> Format(ValidationReport report);
}