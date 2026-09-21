using System.Collections.Concurrent;
using System.Reflection;
using Farewell.Abstractions.Attributes.Validation;
using Farewell.Abstractions.Validation;

namespace Farewell.Application.Validation;

public sealed class AttributeValidatorResolver : IValidatorResolver
{
    private readonly ConcurrentDictionary<(Type, string), Type?> _attributeCache = new();

    public IValidator<T>? Resolve<T>(string context)
    {
        var key = (typeof(T), context);

        var validatorType = _attributeCache.GetOrAdd(key, k =>
        {
            var attrs = k.Item1
                .GetCustomAttributes<ValidatedByAttribute>()
                .Where(a => a.Context == context)
                .ToArray();

            if (attrs.Length > 1)
                throw new InvalidOperationException(
                    $"Multiple validators registered for '{k.Item1.Name}' " +
                    $"with context '{context}'. Only one validator per context is allowed.");

            return attrs.Length == 1 ? attrs[0].ValidatorType : null;
        });

        if (validatorType is null) return null;

        return (IValidator<T>)Activator.CreateInstance(validatorType)!;
    }
}