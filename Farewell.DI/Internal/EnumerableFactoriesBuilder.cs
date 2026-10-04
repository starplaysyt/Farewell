namespace Farewell.DI.Internal;

internal static class EnumerableFactoriesBuilder
{
    internal static Dictionary<Type, Func<IServiceProvider, object>> Build(
        Dictionary<(Type ServiceType, object? Key), List<CompiledService>> compiled)
    {
        var result = new Dictionary<Type, Func<IServiceProvider, object>>();

        var groups = compiled
            .Where(kv => kv.Key.Key is null)
            .ToDictionary(
                kv => kv.Key.ServiceType,
                kv => kv.Value);

        foreach (var (serviceType, services) in groups)
        {
            var elementType = serviceType;
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            
            var svcs = services.ToArray();

            result[enumerableType] = sp =>
            {
                var typed = Array.CreateInstance(elementType, svcs.Length);
                for (var i = 0; i < svcs.Length; i++)
                {
                    var instance = sp switch
                    {
                        ServiceScope scope => scope.ResolveFromCompiledService(svcs[i]),
                        ServiceProvider prov => prov.ResolveFromCompiledService(svcs[i]),
                        _ => throw new InvalidOperationException(
                            "Enumerable resolution requires MyDI ServiceProvider or ServiceScope.")
                    };
                    typed.SetValue(instance, i);
                }

                return typed;
            };
        }

        return result;
    }
}