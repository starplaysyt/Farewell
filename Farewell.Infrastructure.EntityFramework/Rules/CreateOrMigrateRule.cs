using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Logging;
using Farewell.Abstractions.StateRules;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Infrastructure.Rules;

/// <summary>
/// Validates database creation or migration.<br/>
/// Can be used as parent to perform initial configuration of database model - just override TryFixAsync.<br/>
/// Does not execute TryFixAsync when database is migrated.
/// </summary>
/// <param name="context">The DbContext connected to this rule</param>
public class CreateOrMigrateRule(DbContext context, ILogger? logger = null) : IStateRule
{
    public async Task<bool> ValidateAsync(CancellationToken cancellationToken = default)
    {
        var isBaseExists = await context.Database.CanConnectAsync(cancellationToken);

        if (isBaseExists)
        {
            await context.Database.MigrateAsync(cancellationToken);
            return true;
        }
        
        logger?.LogInfo("Creating new database...");

        if (context.Database.GetMigrations().Any())
        {
            await context.Database.MigrateAsync(cancellationToken);
            logger?.LogInfo("Database created through migrations.");
            return false;
        }

        await context.Database.EnsureCreatedAsync(cancellationToken);
        logger?.LogInfo("Database created through EnsureCreated.");

        return false;
    }

    public virtual Task TryFixAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}