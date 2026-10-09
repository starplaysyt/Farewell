using System.Reflection;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Debug.Application;
using Farewell.Debug.Infrastructure.Persistence;
using Farewell.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug.Infrastructure;

public class InitializeInfrastructure
{
    public static void Initialize(IServiceBuilder builder)
    {
        builder.AddCQRSHandlers(Assembly.GetAssembly(typeof(IApplicationMarker)));
        builder.AddMediator();

        builder.AddDefaultDbContext<DefaultDbContext>(options => 
            options.UseAutoConfigurations()
                .UseDomainConventions()
                .UseSqlite());
        builder.AddCreateOrMigrateRule();

        builder.AddDbContext<OtherDbContext>(options =>
            options.UseAutoConfigurations()
                .UseDomainConventions()
                .UseSqlite());
        builder.AddCreateOrMigrateRule<OtherDbContext>();

        builder.AddGenericRepositories();

    }
}