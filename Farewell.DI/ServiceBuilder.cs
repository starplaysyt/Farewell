using Farewell.Abstractions.DI;
using Farewell.DI.Internal;

namespace Farewell.DI;

public sealed class ServiceBuilder : IServiceBuilder
{
    private readonly List<ServiceDescriptor> _descriptors = new();

    internal IReadOnlyList<ServiceDescriptor> Descriptors => _descriptors;

    public ServiceBuilder AddService(ServiceDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _descriptors.Add(descriptor);
        return this;
    }

    public IServiceBuilder AddService(Type serviceType, Type implementationType,
        ServiceLifetimeType serviceLifetime, object? key)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(implementationType);

        _descriptors.Add(ServiceDescriptor.FromType(serviceType, implementationType,
            serviceLifetime, key));
        return this;
    }

    public IServiceBuilder AddService(Type serviceType,
        Func<IServiceProvider, object> implementationFactory,
        ServiceLifetimeType lifetime, object? key)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(implementationFactory);

        _descriptors.Add(
            ServiceDescriptor.FromFactory(serviceType, implementationFactory, lifetime, key));
        return this;
    }

    public IScopeProvider Build()
    {
        // Separating opened and closed generics - different pipeline
        var (openGenericDescriptors, allClosed) = SeparateAndExpand();

        // Compiling delegates for all closed descriptors
        var compiled = ServiceDescriptorCompiler.Compile(allClosed);

        // Graph validation
        GraphValidator.ValidateGraph(compiled.Compiled);

        // Topological singleton sorting
        var singletonOrder = SingletonTopologicalSorter.Sort(compiled.Compiled);

        // Delegates for IEnumerable
        var enumerableFactories = EnumerableFactoriesBuilder.Build(compiled.Compiled);

        // what's the dog doin'?
        return new ServiceProvider(
            compiled.Compiled,
            openGenericDescriptors,
            singletonOrder,
            enumerableFactories, compiled.NextSlotId);
    }

    private (List<ServiceDescriptor> OpenGeneric, List<ServiceDescriptor> AllClosed)
        SeparateAndExpand()
    {
        var openGeneric = new List<ServiceDescriptor>();
        var closed = new List<ServiceDescriptor>();

        foreach (var d in _descriptors)
        {
            if (d.IsOpenGeneric)
                openGeneric.Add(d);
            else
                closed.Add(d);
        }

        // Looking up for closed generics dependencies for open generic registration
        var expanded = new List<ServiceDescriptor>();
        var knownServiceTypes = new HashSet<Type>(closed.Select(d => d.ServiceType));

        var changed = true;
        while (changed)
        {
            changed = false;

            foreach (var d in closed.Concat(expanded))
            {
                if (d.IsFactory || d.ImplementationType is null)
                    continue;

                var ctor = ReflectionHelper.GetSinglePublicConstructor(d.ImplementationType);

                foreach (var param in ctor.GetParameters())
                {
                    var paramType = ReflectionHelper.UnwrapEnumerable(param.ParameterType);

                    if (knownServiceTypes.Contains(paramType))
                        continue;

                    if (!paramType.IsGenericType || paramType.IsGenericTypeDefinition)
                        continue;

                    var genericDef = paramType.GetGenericTypeDefinition();
                    var matchingOpen = openGeneric.FirstOrDefault(og =>
                        og.ServiceType == genericDef && Equals(og.Key, d.Key));

                    if (matchingOpen?.ImplementationType is null)
                        continue;

                    var closedImpl = matchingOpen.ImplementationType
                        .MakeGenericType(paramType.GetGenericArguments());

                    var newDesc = ServiceDescriptor.FromType(
                        paramType, closedImpl, matchingOpen.Lifetime, matchingOpen.Key);

                    expanded.Add(newDesc);
                    knownServiceTypes.Add(paramType);
                    changed = true;
                }
            }
        }

        return (openGeneric, closed.Concat(expanded).ToList());
    }
}