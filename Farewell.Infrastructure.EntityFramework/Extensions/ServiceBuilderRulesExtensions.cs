using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Logging;
using Farewell.Infrastructure.Rules;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Infrastructure.Extensions;

public static class ServiceBuilderRulesExtensions
{
    /// <summary>
    /// Adds CreateOrMigrate rule to the default DbContext.
    /// AddDefaultDbContext should be used to define that default DbContext.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IServiceBuilder AddCreateOrMigrateRule(this IServiceBuilder builder)
    {
        builder.AddStateRule<CreateOrMigrateRule>();
        return builder;
    }

    /// <summary>
    /// Adds CreateOrMigrate rule to provided DbContext.
    /// </summary>
    /// <param name="builder">The IServiceBuilder</param>
    /// <typeparam name="TContext">The DbContext</typeparam>
    /// <returns></returns>
    public static IServiceBuilder AddCreateOrMigrateRule<TContext>(this IServiceBuilder builder)
        where TContext : DbContext
    {
        builder.AddStateRule<CreateOrMigrateRule>((sp) =>
            new CreateOrMigrateRule(sp.GetRequiredService<TContext>(), sp.GetService<ILogger>()));

        return builder;
    }

    /// <summary>
    /// Adds custom CreateOrMigrate rule to provided DbContext.
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="TRule"></typeparam>
    /// <returns></returns>
    public static IServiceBuilder AddCustomCreateOrMigrateRule<TRule>(this IServiceBuilder builder)
        where TRule : CreateOrMigrateRule
    {
        builder.AddStateRule<TRule>();

        return builder;
    }
}