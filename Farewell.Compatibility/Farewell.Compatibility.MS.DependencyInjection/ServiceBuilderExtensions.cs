using Farewell.Abstractions.DI;
using Microsoft.Extensions.DependencyInjection;
using IKeyedServiceProvider = Farewell.Abstractions.DI.IKeyedServiceProvider;

namespace Farewell.Compatibility.MS.DependencyInjection;

public static class ServiceBuilderExtensions
{
    public static IServiceCollection GetServiceCollection(this IServiceBuilder builder)
    {
        return builder is FarewellBuilder frwBuilder 
            ? frwBuilder.ServiceCollection
            : throw new InvalidOperationException("Given IServiceBuilder is not FarewellBuilder.");
    }

    public static IServiceCollection AddKeyedServiceProvider(this IServiceCollection collection)
    {
        collection.AddScoped<IKeyedServiceProvider, FarewellServiceScope>();
        return collection;
    }
}