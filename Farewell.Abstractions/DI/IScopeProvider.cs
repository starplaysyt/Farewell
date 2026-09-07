namespace Farewell.Abstractions.DI;

public interface IScopeProvider : IKeyedServiceProvider
{
    IKeyedServiceProvider CreateScope();
}