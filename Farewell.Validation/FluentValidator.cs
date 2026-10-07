using Farewell.Abstractions.Validation;
using Farewell.Validation.Internal;

namespace Farewell.Validation;

public abstract class FluentValidator<T> : IFluentValidator<T>
{
    private CompiledValidator<T>? _compiled;

    protected FluentValidator() { }

    protected abstract void DefineRules(PropertiesValidatorBuilder<T> builder);

    internal CompiledValidator<T> GetOrCompile()
    {
        if (_compiled is not null) return _compiled;

        var builder = new PropertiesValidatorBuilder<T>();
        DefineRules(builder);
        _compiled = new CompiledValidator<T>(builder.BuildEvaluators());
        return _compiled;
    }

    public ValueTask<ValidationReport> ValidateAsync(
        T instance,
        IServiceProvider serviceProvider,
        IValidationCollector collector,
        CancellationToken ct = default)
        => GetOrCompile().ValidateAsync(instance, serviceProvider, collector, ct);
}