using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Farewell.Infrastructure;

public class DesignDbContextFactory<TContext> : IDesignTimeDbContextFactory<TContext>
    where TContext : DbContext
{
    public static DbContextOptions<TContext>? Options { get; set; }

    public TContext CreateDbContext(string[] args)
    {
        if (Options is null)
            throw new InvalidOperationException(
                $"Unable to get DbContextOptions for {typeof(TContext).FullName} on design time.");

        var ctor = typeof(TContext).GetConstructor([typeof(DbContextOptions<TContext>)]);

        if (ctor is null)
            throw new InvalidOperationException(
                $"{typeof(TContext).FullName} has no constructor with parameter of type {typeof(DbContextOptions<TContext>).FullName})");
        
        return (TContext)ctor.Invoke([Options]);;
    }
}