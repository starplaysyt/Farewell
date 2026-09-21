using System.Collections.Concurrent;
using Farewell.Abstractions.Validation;

namespace Farewell.Application.Validation;

public sealed class ValidationProvider(IValidatorResolver resolver) : IValidationProvider
{
    private readonly ConcurrentDictionary<(Type, string), object?> _fluentCache = new();
    
    private readonly ConcurrentDictionary<(Type, string), object?> _directCache = new();

    public ValidationReport ValidateAll<T>(T instance, string context = "Default")
    {
        var validator = Resolve<T>(context);
        return validator is null
            ? ValidationReport.Ok
            : validator.ValidateAll(instance);
    }

    public ValidationStatus ValidateBreak<T>(T instance, string context = "Default")
    {
        var validator = Resolve<T>(context);
        return validator is null
            ? ValidationStatus.Ok
            : validator.ValidateBreak(instance);
    }

    public void InvalidateCache() => _fluentCache.Clear();

    private IValidator<T>? Resolve<T>(string context)
    {
        var key = (typeof(T), context);
        
        if (_directCache.TryGetValue(key, out var direct))
            return (IValidator<T>?)direct;
        
        if (_fluentCache.TryGetValue(key, out var cached))
            return (IValidator<T>?)cached;
        
        var resolved = resolver.Resolve<T>(context);

        switch (resolved)
        {
            case IDirectValidator<T> directValidator:
                _directCache[key] = directValidator;
                return directValidator;

            case IFluentValidator<T> fluentValidator:
                var compiled = fluentValidator is FluentValidator<T> fv
                    ? fv.GetOrCompile()
                    : null;
                _fluentCache[key] = compiled;
                return compiled;

            default:
                // Validator not found - caching
                _fluentCache[key] = null;
                return null;
        }
    }
}
