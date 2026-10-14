using System.Reflection;
using Farewell.Abstractions.CQRS;
using Farewell.Abstractions.DI;

namespace Farewell.Abstractions.Extensions;

public static class CQRSServiceBuilderExtensions
{
    public static IServiceBuilder AddSingletonMediator(this IServiceBuilder builder)
        => builder.AddSingleton<SingletonMediator>();
    
    public static IServiceBuilder AddMediator(this IServiceBuilder builder)
        => builder.AddScoped<Mediator>();
    
    public static IServiceBuilder AddCQRSHandlers(this IServiceBuilder services, Assembly? assembly, bool doLog = false)
    {
        if (assembly == null)
            throw new InvalidOperationException("Given assembly to AddCQRSHandlers was null.");
        
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new {t , t.BaseType})
            .Where(x => x.BaseType is not null && x.BaseType.IsGenericType && 
                        (x.BaseType.GetGenericTypeDefinition() == typeof(AsyncHandler<,>) ||
                         x.BaseType.GetGenericTypeDefinition() == typeof(AsyncHandler<>) ||
                         x.BaseType.GetGenericTypeDefinition() == typeof(SyncHandler<,>) ||
                         x.BaseType.GetGenericTypeDefinition() == typeof(SyncHandler<>)));

        foreach (var binding in handlerTypes)
        {
            if (doLog) Console.WriteLine($"[ AddCQRSHandlers ] Adding service {binding.BaseType!.FullName} with implementation {binding.BaseType.FullName}.");
            services.AddScoped(binding.BaseType!, binding.t);
        }
        return services;
    }
}