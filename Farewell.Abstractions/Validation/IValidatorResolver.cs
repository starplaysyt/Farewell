namespace Farewell.Abstractions.Validation;

public interface IValidatorResolver
{
    IValidator<T>? Resolve<T>(string context);
}