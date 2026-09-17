namespace Farewell.Abstractions.DI;

public interface IServiceBuilder
{
    public IServiceBuilder AddService(Type serviceType,
        Func<IServiceProvider, object> implementationFactory,
        ServiceLifetimeType lifetime, object? key = null);

    public IServiceBuilder AddService(Type serviceType, Type implementationType,
        ServiceLifetimeType serviceLifetime, object? key = null);

    public IScopeProvider Build();
}