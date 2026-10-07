namespace Farewell.Abstractions.Validation;

public interface IAsyncValidator<in T>
{
    ValueTask<ValidationReport> ValidateAsync(
        T instance,
        IServiceProvider serviceProvider,
        IValidationCollector collector,
        CancellationToken ct = default);
}

/// <summary>
/// Fluent-based validator. Should be registered as Singleton and be stateless.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IFluentValidator<in T> : IAsyncValidator<T> { }

/// <summary>
/// Direct-based validator. Should be registered as Scoped and should resolve its dependencies in-place.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IDirectValidator<in T> : IAsyncValidator<T> { }