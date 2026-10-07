using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Logging;
using Farewell.Debug.ASP.Domain.Entities;
using Farewell.Debug.ASP.Infrastructure.Persistence;
using Farewell.Infrastructure.Rules;

namespace Farewell.Debug.ASP.Infrastructure;

public class PersistenceRules(AppDbContext context, ILogger logger) : CreateOrMigrateRule(context, logger)
{
    private readonly ILogger _logger = logger;

    public override async Task TryFixAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInfo("Fixing...");
        
        context.FirstDomainEntities.Add(new FirstDomainEntity()
            { Field1 = "TestField1", Field2 = "TestField2" });
        
        await context.SaveChangesAsync(cancellationToken);
    }
}