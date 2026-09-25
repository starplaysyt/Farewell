using System.Reflection;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Domain;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Infrastructure;

namespace Farewell.Infrastructure.Extensions;

public static class ServiceBuilderExtensions
{
    public static IServiceBuilder AddGenericRepositories(this IServiceBuilder builder)
    {
        builder.AddService(typeof(IQueryableRepository<,>), typeof(EFRepository<,>),
            ServiceLifetimeType.Scoped);
        return builder;
    }

    public static IServiceBuilder AddImplementedRepositories(this IServiceBuilder builder,
        Assembly applicationAssembly, Assembly infrastructureAssembly)
    {
        var implementations = infrastructureAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false })
            .ToArray();

        var interfaceImplementationPairs = applicationAssembly.GetTypes()
            .Where(t => t is { IsInterface: true, IsGenericTypeDefinition: false } &&
                        t.GetInterfaces().Any(i =>
                            !i.IsGenericType && i.GetGenericTypeDefinition() ==
                            typeof(IQueryableRepository<,>)))
            .Select(iface => (iface,
                implementations.FirstOrDefault(impl => impl.IsAssignableTo(iface))))
            .Where(pair => pair.Item2 is not null)
            .ToArray();

        foreach (var pair in interfaceImplementationPairs)
        {
            builder.AddScoped(pair.iface, pair.Item2!);
        }

        return builder;
    }

    public static IServiceBuilder AddRepository<TService, TImplementation, TEntity, TKey>(
        this IServiceBuilder builder)
        where TService : class, IQueryableRepository<TEntity, TKey>
        where TKey : IComparable<TKey>
        where TEntity : DomainEntity<TKey>
        where TImplementation : EFRepository<TEntity, TKey>, TService
    {
        builder.AddScoped<TService, TImplementation>();
        return builder;
    }
}