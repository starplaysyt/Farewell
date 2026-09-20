using System.Reflection;
using Farewell.Abstractions.DI;
using Farewell.Application.CQRS;

namespace Farewell.Application.Extensions;

public static class ServiceBuilderExtensions
{
    public static IServiceBuilder AddMediator(this IServiceBuilder builder)
    {
        builder.AddScoped<CQRSMediator>();
        return builder;
    }
    
    public static IServiceBuilder AddCQRSHandlers(this IServiceBuilder services, Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces(), (impl, iface) => new { impl, iface })
            .Where(x => x.iface.IsGenericType && 
                        (x.iface.GetGenericTypeDefinition() == typeof(CQRSAsyncHandler<,>) ||
                         x.iface.GetGenericTypeDefinition() == typeof(CQRSAsyncHandler<>) ||
                         x.iface.GetGenericTypeDefinition() == typeof(CQRSSyncHandler<,>) ||
                         x.iface.GetGenericTypeDefinition() == typeof(CQRSSyncHandler<>)));

        foreach (var binding in handlerTypes)
        {
            services.AddScoped(binding.iface, binding.impl);
        }
        return services;
    }
}