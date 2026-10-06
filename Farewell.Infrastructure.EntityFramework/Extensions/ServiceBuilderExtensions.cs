using System.Reflection;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Domain;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Infrastructure.Extensions;

public static class ServiceBuilderExtensions
{
    // INFO: Probably there is better way of adding DBContext to DI
    public static IServiceBuilder AddDefaultDbContext<TContext>(this IServiceBuilder builder)
        where TContext : DbContext
    {
        builder.AddScoped<DbContext, TContext>();
        return builder;
    }
    
    public static IServiceBuilder AddDbContext<TContext>(this IServiceBuilder builder)
        where TContext : DbContext
    {
        builder.AddScoped<TContext>();
        return builder;
    }

    /// <summary>
    /// Adds default-implemented repository for TEntity with manually selected DbContext.
    /// </summary>
    /// <param name="builder">Service builder</param>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TContext">Selected DbContext</typeparam>
    /// <returns></returns>
    public static IServiceBuilder AddDefaultRepository<TEntity, TContext>(
        this IServiceBuilder builder) 
        where TEntity : DomainEntity 
        where TContext : DbContext
    {
        builder.AddScoped<IQueryableRepository<TEntity>, EFRepository<TEntity, TContext>>();
        return builder;
    }

    /// <summary>
    /// Adds default-implemented repository for TEntity, connected to the default DbContext.
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IServiceBuilder AddDefaultRepository<TEntity>(this IServiceBuilder builder) 
        where TEntity : DomainEntity
    {
        builder.AddScoped<IQueryableRepository<TEntity>, EFRepository<TEntity, DbContext>>();
        return builder;
    }

    /// <summary>
    /// Adds custom queryable implementation as IQueryableRepository.
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <returns></returns>
    public static IServiceBuilder AddQueryableRepository<TEntity, TImplementation>(this IServiceBuilder builder) 
        where TEntity : DomainEntity
        where TImplementation : class, IQueryableRepository<TEntity>
    {
        builder.AddScoped<IQueryableRepository<TEntity>, TImplementation>();
        return builder;
    }

    /// <summary>
    /// Adds repository with specified interface and implementation.
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="TRepository"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <returns></returns>
    public static IServiceBuilder AddRepository<TRepository, TImplementation>(
        this IServiceBuilder builder)
        where TImplementation : class, 
        TRepository where TRepository : class
    {
        builder.AddScoped<TRepository, TImplementation>();
        return builder;
    }
    
    /// <summary>
    /// Adds generic-implemented repositories, can be called from services as IQueryableRepository&ltTEntity&gt.
    /// AddDefaultDbContext call required - all generic repositories will be connected to default DbContext
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IServiceBuilder AddGenericRepositories(this IServiceBuilder builder)
    {
        builder.AddService(typeof(IQueryableRepository<>), typeof(EFRepository<>), 
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
                            typeof(IQueryableRepository<>)))
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
        where TService : class, IQueryableRepository<TEntity>
        where TKey : IComparable<TKey>
        where TEntity : DomainEntity<TKey>
        where TImplementation : EFRepository<TEntity>, TService
    {
        builder.AddScoped<TService, TImplementation>();
        return builder;
    }
}