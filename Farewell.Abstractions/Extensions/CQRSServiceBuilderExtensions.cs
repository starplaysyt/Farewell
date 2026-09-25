using System.Reflection;
using Farewell.Abstractions.CQRS;
using Farewell.Abstractions.DI;

namespace Farewell.Abstractions.Extensions;

public static class CQRSServiceBuilderExtensions
{
    public static IServiceBuilder AddMediator(this IServiceBuilder builder)
    {
        builder.AddScoped<Mediator>();
        return builder;
    }
    
    public static IServiceBuilder AddCQRSHandlers(this IServiceBuilder services, Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces(), (impl, iface) => new { impl, iface })
            .Where(x => x.iface.IsGenericType && 
                        (x.iface.GetGenericTypeDefinition() == typeof(AsyncHandler<,>) ||
                         x.iface.GetGenericTypeDefinition() == typeof(AsyncHandler<>) ||
                         x.iface.GetGenericTypeDefinition() == typeof(SyncHandler<,>) ||
                         x.iface.GetGenericTypeDefinition() == typeof(SyncHandler<>)));

        foreach (var binding in handlerTypes)
        {
            services.AddScoped(binding.iface, binding.impl);
        }
        return services;
    }
}