using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Farewell.Infrastructure.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    extension(DbContextOptionsBuilder optionsBuilder)
    {
        public DbContextOptionsBuilder UseDomainConventions()
        {
            var extension = optionsBuilder.Options
                                .FindExtension<DomainConventionsOptionsExtension>()
                            ?? new DomainConventionsOptionsExtension();

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                .AddOrUpdateExtension(extension);

            return optionsBuilder;
        }

        public DbContextOptionsBuilder UseAutoConfigurations(Assembly? assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            var extension = optionsBuilder.Options
                                .FindExtension<AutoConfigurationOptionsExtension>()
                            ?? new AutoConfigurationOptionsExtension(assembly);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                .AddOrUpdateExtension(extension);

            return optionsBuilder;
        }

        public DbContextOptionsBuilder UseAutoConfigurations(string namespaceName)
        {
            var assembly = Assembly.LoadFrom(namespaceName);
            return optionsBuilder.UseAutoConfigurations(assembly);
        }
    }
}