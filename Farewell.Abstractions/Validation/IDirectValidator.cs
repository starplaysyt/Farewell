namespace Farewell.Abstractions.Validation;

public interface IDirectValidator<in T> : IAsyncValidator<T>;