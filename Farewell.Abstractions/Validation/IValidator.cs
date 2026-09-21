namespace Farewell.Abstractions.Validation;

// General validator for T
public interface IValidator<in T>
{
    ValidationReport ValidateAll(T instance);
    ValidationStatus ValidateBreak(T instance);
}

public interface IFluentValidator<in T> : IValidator<T>
{
}

public interface IDirectValidator<in T> : IValidator<T>
{
}