using Microsoft.Extensions.DependencyInjection;

namespace Farewell.Compatibility.MS.DependencyInjection;

public class FarewellScopeProvider(IServiceProvider serviceProvider) : Abstractions.DI.IScopeProvider
{
    public object? GetService(Type serviceType)
    {
        return serviceProvider.GetService(serviceType);
    }

    public void Dispose()
    {
        if (serviceProvider is IServiceScope scopeProvider)
            scopeProvider.Dispose();
        
        GC.SuppressFinalize(this);
    }

    public object? GetKeyedService(Type serviceType, object key)
    {
        return serviceProvider.GetKeyedService(serviceType, key);
    }

    public Abstractions.DI.IKeyedServiceProvider CreateScope()
    {
        return new FarewellServiceScope(serviceProvider.CreateScope());
    }
}