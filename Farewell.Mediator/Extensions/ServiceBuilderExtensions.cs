using System.Reflection;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Mediator;

namespace Farewell.Mediator.Extensions;

public static class ServiceBuilderExtensions
{
    /// <summary>
    /// Scans given assembly and adds all classes, derived from AsyncHandler to DI-container.
    /// </summary>
    /// <param name="builder">Given IServiceBuilder.</param>
    /// <param name="assembly">An Assembly to scan.</param>
    /// <param name="doLog">Do log of all added Mediator handlers.</param>
    /// <returns>Given IServiceBuilder.</returns>
    /// <exception cref="InvalidOperationException">Throws when given assembly is null.</exception>
    public static IServiceBuilder AddMediatorHandlers(this IServiceBuilder builder, Assembly? assembly, bool doLog = false)
    {
        if (assembly == null)
            throw new ArgumentException("Given assembly was null.");

        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new { t, t.BaseType })
            .Where(x => x.BaseType is not null && x.BaseType.IsGenericType &&
                        (x.BaseType.GetGenericTypeDefinition() == typeof(AsyncHandler<,>) ||
                         x.BaseType.GetGenericTypeDefinition() == typeof(AsyncHandler<>)));

        foreach (var binding in handlerTypes)
        {
            if (doLog) Console.WriteLine($"[ AddMediatorHandlers ] Adding service {binding.BaseType!.FullName} with implementation {binding.t.FullName}.");
            builder.AddScoped(binding.BaseType!, binding.t);
        }
        return builder;
    }
    
    /// <summary>
    /// Registers IMediator service as Mediator.
    /// </summary>
    /// <param name="builder">Given IServiceBuilder.</param>
    /// <returns>Given IServiceBuilder.</returns>
    public static IServiceBuilder AddMediator(this IServiceBuilder builder)
    {
        builder.AddScoped<IMediator, Mediator>();
        return builder;
    }

    /// <summary>
    /// Registers custom IMediator service.
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="TMediator"></typeparam>
    /// <returns></returns>
    public static IServiceBuilder AddCustomMediator<TMediator>(this IServiceBuilder builder)
        where TMediator : class, IMediator
    {
        builder.AddScoped<IMediator, TMediator>();
        return builder;
    }
}