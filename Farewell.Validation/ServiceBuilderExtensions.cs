using System.Reflection;
using Farewell.Abstractions.Attributes.Validation;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Validation;
using Farewell.Validation.Internal;

namespace Farewell.Validation;

public static class ServiceBuilderExtensions
{
    public static IServiceBuilder AddValidation(this IServiceBuilder services)
    {
        services.AddScoped<IValidationProvider, ValidationProvider>();
        return services;
    }

    public static IServiceBuilder AddValidators(
        this IServiceBuilder services, Assembly assembly, bool doLog = false)
    {
        var fluentTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } &&
                        ImplementsGenericInterface(t, typeof(IFluentValidator<>)));

        foreach (var type in fluentTypes)
        {
            var entityType = GetGenericArgument(type, typeof(IFluentValidator<>))!;
            var context = GetValidationContext(type);
            RegisterFluentValidator(services, entityType, type, context);
        }

        var directTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } &&
                        ImplementsGenericInterface(t, typeof(IDirectValidator<>)));

        foreach (var type in directTypes)
        {
            var entityType = GetGenericArgument(type, typeof(IDirectValidator<>))!;
            var context = GetValidationContext(type);
            RegisterDirectValidator(services, entityType, type, context);
        }

        return services;
    }

    public static IServiceBuilder AddValidator<TValidator, TEntity>(
        this IServiceBuilder services, string context = "Default")
        where TValidator : class, IFluentValidator<TEntity>
    {
        RegisterFluentCore<TEntity, TValidator>(services, context);
        return services;
    }

    private static void RegisterFluentValidator(
        IServiceBuilder services, Type entityType, Type validatorType, string context)
    {
        typeof(ServiceBuilderExtensions)
            .GetMethod(nameof(RegisterFluentCore), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(entityType, validatorType)
            .Invoke(null, [services, context]);
    }

    private static void RegisterDirectValidator(
        IServiceBuilder services, Type entityType, Type validatorType, string context)
    {
        typeof(ServiceBuilderExtensions)
            .GetMethod(nameof(RegisterDirectCore), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(entityType, validatorType)
            .Invoke(null, [services, context]);
    }

    private static void RegisterFluentCore<TEntity, TValidator>(
        IServiceBuilder services, string context)
        where TValidator : class, IFluentValidator<TEntity>
    {
        services.AddSingleton<TValidator>();

        services.AddKeyedSingleton<IAsyncValidator<TEntity>>(context, (sp) =>
        {
            var validator = sp.GetRequiredService<TValidator>();
            var builder = new ConstraintBuilder<TEntity>();
            validator.Initialize(builder);
            return validator;
        });
    }

    private static void RegisterDirectCore<TEntity, TValidator>(
        IServiceBuilder services, string context)
        where TValidator : class, IDirectValidator<TEntity>
    {
        services.AddScoped<TValidator>();

        services.AddKeyedScoped<IAsyncValidator<TEntity>>(context,
            (sp) => sp.GetRequiredService<TValidator>());
    }


    private static string GetValidationContext(Type type)
    {
        var attr = type.GetCustomAttribute<ValidationContextAttribute>();
        return attr?.Context ?? "Default";
    }

    private static bool ImplementsGenericInterface(Type type, Type genericInterface)
        => type.GetInterfaces().Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface);

    private static Type? GetGenericArgument(Type type, Type genericInterface)
        => type.GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface)
            ?.GetGenericArguments()[0];
}