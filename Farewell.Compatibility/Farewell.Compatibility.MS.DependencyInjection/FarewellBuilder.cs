using Farewell.Abstractions.DI;
using Microsoft.Extensions.DependencyInjection;

namespace Farewell.Compatibility.MS.DependencyInjection;

public class FarewellBuilder(IServiceCollection collection) : IServiceBuilder
{
    public IServiceCollection ServiceCollection => collection;
    
    public IServiceBuilder AddService(Type serviceType, Func<IServiceProvider, object> implementationFactory,
        ServiceLifetimeType lifetime, object? key = null)
    {
        collection.Add(new ServiceDescriptor(serviceType, key, DescriptorFactory, (ServiceLifetime)lifetime));

        return this;
        
        object DescriptorFactory(IServiceProvider provider, object? curKey) 
            => implementationFactory(provider);
    }

    public IServiceBuilder AddService(Type serviceType, Func<IServiceProvider, object?, object> implementationFactory,
        ServiceLifetimeType lifetime, object? key = null)
    {
        collection.Add(new ServiceDescriptor(serviceType, key, implementationFactory, (ServiceLifetime)lifetime));
        
        return this;
    }

    public IServiceBuilder AddService(Type serviceType, Type implementationType,
        ServiceLifetimeType serviceLifetime, object? key = null)
    {
        collection.Add(new ServiceDescriptor(serviceType, key, implementationType, (ServiceLifetime)serviceLifetime));
        return this;
    }

    public IScopeProvider Build()
    {
        return new FarewellScopeProvider(collection.BuildServiceProvider());
    }
}