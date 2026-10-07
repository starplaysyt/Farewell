using System.Reflection;
using Farewell.Abstractions.Attributes.Validation;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Exceptions;
using Farewell.Abstractions.Validation;

namespace Farewell.Abstractions.Extensions;

public static class ValidationServiceBuilderExtensions
{
    public static IServiceBuilder AddValidators(
        this IServiceBuilder services,
        params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
            ScanAndRegister(services, assembly);

        return services;
    }

    public static IServiceBuilder AddValidator<TValidator>(
        this IServiceBuilder services,
        string context = "Default")
        where TValidator : class
    {
        var (serviceType, isFluent) = ResolveValidatorInfo(typeof(TValidator));
        
        if (serviceType is null) 
            throw new DIInvalidImplementation(typeof(TValidator), typeof(IFluentValidator<>), typeof(IDirectValidator<>));
        
        services.AddService(serviceType, typeof(TValidator),
            isFluent ? ServiceLifetimeType.Singleton : ServiceLifetimeType.Scoped, context);

        return services;
    }

    private static void ScanAndRegister(IServiceBuilder services, Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface) continue;

            var (serviceType, isFluent) = ResolveValidatorInfo(type);
            if (serviceType is null) continue;

            var context = type.GetCustomAttribute<ValidationContextAttribute>()
                ?.Context ?? "Default";

            services.AddService(serviceType, type,
                isFluent ? ServiceLifetimeType.Singleton : ServiceLifetimeType.Scoped, context);
        }
    }

    private static (Type? serviceType, bool isFluent) ResolveValidatorInfo(Type type)
    {
        foreach (var service in type.GetInterfaces())
        {
            if (!service.IsGenericType) continue;
            var def = service.GetGenericTypeDefinition();

            if (def == typeof(IFluentValidator<>))
                return (typeof(IAsyncValidator<>).MakeGenericType(service.GenericTypeArguments),
                    true);

            if (def == typeof(IDirectValidator<>))
                return (typeof(IAsyncValidator<>).MakeGenericType(service.GenericTypeArguments),
                    false);
        }

        return (null, false);
    }
}