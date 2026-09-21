using Farewell.Abstractions.Validation;
using Farewell.Application.Validation.Internal;

namespace Farewell.Application.Validation;

public abstract class FluentValidator<T> : IFluentValidator<T>
{
    private CompiledValidator<T>? _compiled;

    internal CompiledValidator<T> GetOrCompile()
    {
        if (_compiled is not null) return _compiled;

        var builder = new PropertiesValidatorBuilder<T>();
        DefineRules(builder);
        var evaluators = builder.BuildEvaluators();
        _compiled = ValidatorCompiler.Compile(evaluators);
        return _compiled;
    }

    protected abstract void DefineRules(PropertiesValidatorBuilder<T> builder);

    public ValidationReport ValidateAll(T instance)
        => GetOrCompile().ValidateAll(instance);

    public ValidationStatus ValidateBreak(T instance)
        => GetOrCompile().ValidateBreak(instance);
}