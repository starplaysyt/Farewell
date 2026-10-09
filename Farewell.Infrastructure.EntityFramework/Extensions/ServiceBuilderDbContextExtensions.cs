using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Infrastructure.Extensions;

public static class ServiceBuilderDbContextExtensions
{
    /// <summary>
    /// Adds default <c>DbContext</c> with parameterless constructor. That means that you need to
    /// overload <c>OnConfiguring</c> method in your <c>DbContext</c> inheritor.
    /// </summary>
    /// <param name="builder">Given <c>IServiceBuilder</c>.</param>
    /// <typeparam name="TContext">Inheritor of the <c>DbContext</c></typeparam>
    /// <remarks>
    /// Adds as services <c>TContext</c>, <c>DbContext</c>.<br/>
    /// </remarks>
    /// <returns>Given <c>IServiceBuilder</c>.</returns>
    public static IServiceBuilder AddDefaultDbContext<TContext>(this IServiceBuilder builder)
        where TContext : DbContext
    {
        builder.AddScoped<DbContext, TContext>();
        builder.AddScoped<TContext>();
        return builder;
    }

    /// <summary>
    /// Adds default <c>DbContext</c> with constructor, that accepts <c>DbContextOptions&lt;TContext&gt;</c>.
    /// </summary>
    /// <param name="builder">Given <c>IServiceBuilder</c>.</param>
    /// <param name="optionsBuilderAction">Delegate, what adds options to <c>DbContextOptionsBuilder</c>.</param>
    /// <typeparam name="TContext">Inheritor of the <c>DbContext</c></typeparam>
    /// <remarks>
    /// Adds as services <c>TContext</c>, <c>DoContextOptions&lt;TContext&gt;</c>, <c>DbContext</c>.<br/>
    /// Also initializes Options in <c>DesignDbContextFactory&lt;TContext&gt;</c>.
    /// </remarks>
    /// <returns>Given <c>IServiceBuilder</c>.</returns>
    public static IServiceBuilder AddDefaultDbContext<TContext>(
        this IServiceBuilder builder,
        Action<DbContextOptionsBuilder<TContext>> optionsBuilderAction) 
        where TContext : DbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        optionsBuilderAction(optionsBuilder);
        DesignDbContextFactory<TContext>.Options = optionsBuilder.Options;

        builder.AddSingleton((_) => optionsBuilder.Options);
        builder.AddScoped<DbContext, TContext>();
        builder.AddScoped<TContext>();

        return builder;
    }

    /// <summary>
    /// Adds <c>DbContext</c> inheritor with parameterless constructor. That means that you need to
    /// overload <c>OnConfiguring</c> method in your <c>DbContext</c> inheritor.
    /// </summary>
    /// <param name="builder">Given <c>IServiceBuilder</c>.</param>
    /// <typeparam name="TContext">Inheritor of the <c>DbContext</c></typeparam>
    /// <remarks>
    /// Adds as services <c>TContext</c>.<br/>
    /// </remarks>
    /// <returns>Given <c>IServiceBuilder</c>.</returns>
    public static IServiceBuilder AddDbContext<TContext>(this IServiceBuilder builder)
        where TContext : DbContext
    {
        builder.AddScoped<TContext>();
        return builder;
    }
    
    /// <summary>
    /// Adds <c>DbContext</c> inheritor with constructor, that accepts <c>DbContextOptions&lt;TContext&gt;</c>.
    /// </summary>
    /// <param name="builder">Given <c>IServiceBuilder</c>.</param>
    /// <param name="optionsBuilderAction">Delegate, what adds options to <c>DbContextOptionsBuilder</c>.</param>
    /// <typeparam name="TContext">Inheritor of the <c>DbContext</c></typeparam>
    /// <remarks>
    /// Adds as services <c>TContext</c>, <c>DoContextOptions&lt;TContext&gt;</c>.<br/>
    /// Also initializes Options in <c>DesignDbContextFactory&lt;TContext&gt;</c>.
    /// </remarks>
    /// <returns>Given <c>IServiceBuilder</c>.</returns>
    public static IServiceBuilder AddDbContext<TContext>(
        this IServiceBuilder builder,
        Action<DbContextOptionsBuilder<TContext>> optionsBuilderAction) 
        where TContext : DbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        optionsBuilderAction(optionsBuilder);
        DesignDbContextFactory<TContext>.Options = optionsBuilder.Options;
        
        builder.AddSingleton((_) => optionsBuilder.Options);
        builder.AddScoped<TContext>();

        return builder;
    }
}