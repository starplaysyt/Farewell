using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Farewell.Infrastructure.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseDomainConventions(
        this DbContextOptionsBuilder optionsBuilder)
    {
        var extension = optionsBuilder.Options
                            .FindExtension<DomainConventionsOptionsExtension>()
                        ?? new DomainConventionsOptionsExtension();

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
            .AddOrUpdateExtension(extension);

        return optionsBuilder;
    }
}