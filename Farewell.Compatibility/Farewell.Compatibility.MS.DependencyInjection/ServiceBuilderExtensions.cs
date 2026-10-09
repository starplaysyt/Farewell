using Farewell.Abstractions.DI;
using Microsoft.Extensions.DependencyInjection;

namespace Farewell.Compatibility.MS.DependencyInjection;

public static class ServiceBuilderExtensions
{
    public static IServiceCollection GetServiceCollection(this IServiceBuilder builder)
    {
        return builder is FarewellBuilder frwBuilder 
            ? frwBuilder.ServiceCollection
            : throw new InvalidOperationException("Given IServiceBuilder is not FarewellBuilder.");
    }
}