using System.Reflection;

namespace Farewell.DI.Internal;

internal class ServiceDescriptorCompiler
{
    internal static (Dictionary<(Type, object?), List<CompiledService>> Compiled, int NextSlotId)
        Compile(List<ServiceDescriptor> descriptors)
    {
        var result = new Dictionary<(Type, object?), List<CompiledService>>();
        var slotCounter = 0;

        foreach (var d in descriptors)
        {
            var dictKey = (d.ServiceType, d.Key);
            if (!result.ContainsKey(dictKey))
                result[dictKey] = [];

            Type implType;
            Func<IServiceProvider, object> factory;
            Type[] depTypes;
            object?[] depKeys;

            if (d.IsFactory)
            {
                implType = d.ServiceType;
                factory = d.Factory!;
                depTypes = [];
                depKeys = [];
            }
            else
            {
                implType = d.ImplementationType!;
                var ctor = ReflectionHelper.GetSinglePublicConstructor(implType);
                var parameters = ctor.GetParameters();

                depTypes = new Type[parameters.Length];
                depKeys = new object?[parameters.Length];

                for (var i = 0; i < parameters.Length; i++)
                {
                    var keyAttr = parameters[i].GetCustomAttribute<FromKeyedServicesAttribute>();
                    depTypes[i] = ReflectionHelper.UnwrapEnumerable(parameters[i].ParameterType);
                    depKeys[i] = keyAttr?.Key;
                }

                factory = ExpressionFactoryCompiler.Compile(ctor, parameters);
            }

            result[dictKey].Add(new CompiledService
            {
                SlotId = slotCounter++,
                ServiceType = d.ServiceType,
                ImplementationType = implType,
                Lifetime = d.Lifetime,
                Key = d.Key,
                Factory = factory,
                DependencyTypes = depTypes,
                DependencyKeys = depKeys
            });
        }

        return (result, slotCounter);
    }
}