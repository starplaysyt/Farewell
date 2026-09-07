namespace Farewell.Abstractions.DI;

public interface IKeyedServiceProvider : IServiceProvider, IDisposable
{
    object? GetKeyedService(Type serviceType, object key);
}