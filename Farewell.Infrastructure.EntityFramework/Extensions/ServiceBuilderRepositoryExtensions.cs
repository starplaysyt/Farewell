using System.Reflection;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Domain;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Infrastructure.Extensions;

public static class ServiceBuilderRepositoryExtensions
{
    #region Auto Adders

    /// <summary>
    /// Adds <c>IQueryableRepository&lt;TEntity&gt;</c> for every reflected DomainEntity(TEntity) in
    /// specified assembly and namespace with specified <c>DbContext</c>.
    /// If namespace is not specified, will look through all assembly.
    /// </summary>
    /// <param name="builder">Provided <c>IServiceBuilder</c>.</param>
    /// <param name="assembly">Assembly to perform search.</param>
    /// <param name="namespace">Entities namespace to lookup.</param>
    /// <param name="includeNestedNamespaces">Do nested types search.</param>
    /// <param name="logAdded">Do log added repositories.</param>
    /// <typeparam name="TContext">Specific DbContext for created repositories.</typeparam>
    /// <returns>Provided <c>IServiceBuilder</c>.</returns>
    public static IServiceBuilder AddAutoRepositories<TContext>(this IServiceBuilder builder,
        Assembly assembly, string? @namespace = null,
        bool includeNestedNamespaces = false, bool logAdded = false)
        where TContext : DbContext
    {
        var types = assembly.GetTypes();

        var selectedTypes =
            types.Where(t =>
                t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(DomainEntity)));

        var domainCheck = @namespace == null
            ? selectedTypes
            : includeNestedNamespaces
                ? selectedTypes.Where(t => t.Namespace?.StartsWith(@namespace) ?? false)
                : selectedTypes.Where(t => t.Namespace == @namespace);

        var foundEntities = domainCheck.ToArray();
        var queryableType = typeof(IQueryableRepository<>);
        var efRepositoryType = typeof(EFRepository<,>);
        var contextType = typeof(TContext);

        foreach (var entityType in foundEntities)
        {
            var genericEntityType = queryableType.MakeGenericType(entityType);
            var genericRepoType = efRepositoryType.MakeGenericType(entityType, contextType);

            builder.AddScoped(genericEntityType, genericRepoType);

            if (logAdded)
                Console.WriteLine("[AddAutoRepositories] Adding Repository {0}<{1}> of type {2}<{3}, {4}>",
                    genericEntityType.Name, entityType.Name, efRepositoryType.Name, entityType.Name, contextType.Name);
        }

        return builder;
    }

    /// <summary>
    /// Adds <c>IQueryableRepository&lt;TEntity&gt;</c> for every reflected DomainEntity(TEntity) in
    /// specified assembly and namespace with <c>DbContext</c> context. That one can be registered with
    /// <c>AddDefaultDbContext</c> method.
    /// If namespace is not specified, will look through all assembly.
    /// </summary>
    /// <param name="builder">Provided <c>IServiceBuilder</c>.</param>
    /// <param name="assembly">Assembly to perform search.</param>
    /// <param name="namespace">Entities namespace to lookup.</param>
    /// <param name="includeNestedNamespaces">Do nested types search.</param>
    /// <param name="logAdded">Do log added repositories.</param>
    /// <returns>Provided <c>IServiceBuilder</c>.</returns>
    public static IServiceBuilder AddAutoRepositories(this IServiceBuilder builder,
        Assembly assembly, string? @namespace = null,
        bool includeNestedNamespaces = false, bool logAdded = false)
        => builder.AddAutoRepositories<DbContext>(assembly, @namespace: @namespace,
            includeNestedNamespaces: includeNestedNamespaces, logAdded);

    #endregion

    #region Generic Repositories

    /// <summary>
    /// Adds default <c>IQueryableRepository</c> for specific TEntity with manually selected DbContext.
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

    #endregion


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
    public static IServiceBuilder AddQueryableRepository<TEntity, TImplementation>(
        this IServiceBuilder builder)
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
        TRepository
        where TRepository : class
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