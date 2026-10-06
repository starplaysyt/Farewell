using Microsoft.EntityFrameworkCore;

namespace Farewell.Infrastructure.Seeding;

public class SeedingContainer
{
    private Dictionary<Type, Action<IServiceProvider, DbContext>> Seedings { get; set; } =
        new();

    public void AddSeedingRule<TContext>(Action<IServiceProvider, DbContext> rule)
        where TContext : DbContext
    {
        Seedings.TryAdd(typeof(TContext), rule);
    }

    public SeedingContainer()
    {
        
    }
}