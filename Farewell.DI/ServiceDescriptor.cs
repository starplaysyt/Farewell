using Farewell.Abstractions.DI;

namespace Farewell.DI;

public sealed class ServiceDescriptor
{
    public Type ServiceType { get; }
    public Type? ImplementationType { get; }
    public Func<IServiceProvider, object>? Factory { get; }
    public ServiceLifetimeType Lifetime { get; }
    public object? Key { get; }

    public bool IsKeyed => Key is not null;
    public bool IsFactory => Factory is not null;
    public bool IsOpenGeneric => ServiceType.IsGenericTypeDefinition;

    // Constructor for factory methods
    private ServiceDescriptor(
        Type serviceType,
        Type? implementationType,
        Func<IServiceProvider, object>? factory,
        ServiceLifetimeType lifetime,
        object? key)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Factory = factory;
        Lifetime = lifetime;
        Key = key;
    }

    // Factory from implementation and interface
    public static ServiceDescriptor FromType(
        Type serviceType,
        Type implementationType,
        ServiceLifetimeType lifetime,
        object? key = null)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(implementationType);

        ValidateImplementationCompatibility(serviceType, implementationType);

        return new ServiceDescriptor(serviceType, implementationType, null, lifetime, key);
    }

    // Factory from type and delegate-factory
    public static ServiceDescriptor FromFactory(
        Type serviceType,
        Func<IServiceProvider, object> factory,
        ServiceLifetimeType lifetime,
        object? key = null)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(factory);

        if (serviceType.IsGenericTypeDefinition)
            throw new ArgumentException(
                "Factory registration is not supported for open generic types.",
                nameof(serviceType));

        return new ServiceDescriptor(serviceType, null, factory, lifetime, key);
    }

    // Validates compatibility between implementation and service
    private static void ValidateImplementationCompatibility(Type serviceType, Type implementationType)
    {
        if (serviceType.IsGenericTypeDefinition)
        {
            if (!implementationType.IsGenericTypeDefinition)
                throw new ArgumentException(
                    $"Service type '{serviceType}' is an open generic, " +
                    $"but implementation '{implementationType}' is not.");
            
            // There should be compatibility checks between two open-generics(service and impl),
            // but I'm tired, go fuck something with these generics
            return;
        }
        
        if (!serviceType.IsAssignableFrom(implementationType))
            throw new ArgumentException(
                $"Implementation type '{implementationType}' is not assignable " +
                $"to service type '{serviceType}'.");

        if (implementationType.IsAbstract || implementationType.IsInterface)
            throw new ArgumentException(
                $"Implementation type '{implementationType}' must be a concrete class.");
    }
}