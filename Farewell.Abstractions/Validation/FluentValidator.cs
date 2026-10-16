namespace Farewell.Abstractions.Validation;

public abstract class FluentValidator<T> : IFluentValidator<T>
{
    private ICompiledValidator<T>? _compiled;

    void IFluentValidator<T>.Initialize(IConstraintBuilder<T> builder)
    {
        DefineRules(builder);
        _compiled = builder.Compile();
    }

    protected abstract void DefineRules(IConstraintBuilder<T> builder);

    public ValueTask<ValidationReport> ValidateAsync(
        T instance,
        IServiceProvider serviceProvider,
        IValidationCollector collector,
        CancellationToken ct = default)
        => _compiled?.ValidateAsync(instance, serviceProvider, collector, ct)
           ?? throw new InvalidOperationException(
               $"Validator {GetType().Name} was not initialized. " +
               "Ensure it is registered via AddValidators() or AddValidator<T>().");
}