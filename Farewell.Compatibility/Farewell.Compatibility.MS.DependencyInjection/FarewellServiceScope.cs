using Microsoft.Extensions.DependencyInjection;

namespace Farewell.Compatibility.MS.DependencyInjection;

public class FarewellServiceScope(IServiceScope scope) : Abstractions.DI.IKeyedServiceProvider, IKeyedServiceProvider
{
    public object? GetService(Type serviceType)
    {
        return scope.ServiceProvider.GetService(serviceType);
    }

    public void Dispose()
    {
        scope.Dispose();
        GC.SuppressFinalize(this);
    }

    public object? GetKeyedService(Type serviceType, object? key)
    {
        return scope.ServiceProvider.GetKeyedService(serviceType, key);
    }

    public object GetRequiredKeyedService(Type serviceType, object? serviceKey)
    {
        return scope.ServiceProvider.GetRequiredKeyedService(serviceType, serviceKey);
    }
}