namespace Farewell.Abstractions.Validation;

public interface IFluentValidator<T> : IAsyncValidator<T>
{
    void Initialize(IConstraintBuilder<T> builder);
}