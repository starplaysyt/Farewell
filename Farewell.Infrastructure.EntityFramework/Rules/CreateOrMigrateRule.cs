using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Logging;
using Farewell.Abstractions.StateRules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

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
        var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
        
        var hasMigrations = context.Database.GetMigrations().Any();
        
        var dbExists = await databaseCreator.ExistsAsync(cancellationToken).ConfigureAwait(false);
        if (!dbExists)
        {
            logger?.LogWarn($"Database for context {context.GetType().FullName} does not exist.", "CreateOrMigrateRule");
            return false;
        }
        
        if (hasMigrations)
        {
            var pendingMigrations = await context.Database
                .GetPendingMigrationsAsync(cancellationToken)
                .ConfigureAwait(false);

            if (pendingMigrations.Any())
            {
                logger?.LogWarn($"Database for context {context.GetType().FullName} exists, but has pending migrations.", "CreateOrMigrateRule");
                return false;
            }
        }
        else
        {
            var hasTables = await databaseCreator.HasTablesAsync(cancellationToken).ConfigureAwait(false);
            if (!hasTables)
            {
                logger?.LogWarn($"Database file for context {context.GetType().FullName} exists, but it is empty (no tables).", "CreateOrMigrateRule");
                return false;
            }
        }

        logger?.LogInfo($"Database for context {context.GetType().FullName} state is valid and up-to-date.", "CreateOrMigrateRule");
        return true;
    }

    public async Task TryFixAsync(CancellationToken cancellationToken = default)
    {
        logger?.LogInfo($"Attempting to fix database for context {context.GetType().FullName} state...", "CreateOrMigrateRule");
        
        var hasMigrations = context.Database.GetMigrations().Any();

        if (hasMigrations)
        {
            logger?.LogInfo("Applying migrations...");
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            logger?.LogInfo("No migrations found. Creating database tables using EnsureCreated...", "CreateOrMigrateRule");
            await context.Database.EnsureCreatedAsync(cancellationToken).ConfigureAwait(false);
        }

        logger?.LogInfo($"Database state for context {context.GetType().FullName} successfully fixed.", "CreateOrMigrateRule");
    }
}