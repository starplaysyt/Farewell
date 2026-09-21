namespace Farewell.Abstractions.Validation;

public interface IValidationMessageResolver
{
    string Resolve(ValidationCode code, string propertyName);
}