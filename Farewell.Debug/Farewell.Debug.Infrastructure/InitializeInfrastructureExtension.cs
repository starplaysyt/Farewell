using System.Reflection;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Debug.Application;
using Farewell.Debug.Domain;
using Farewell.Debug.Infrastructure.Persistence;
using Farewell.Infrastructure.Extensions;
using Farewell.Logging;
using Farewell.StateRules;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug.Infrastructure;

public static class InitializeInfrastructureExtension
{
    public static IServiceBuilder AddInfrastructure(this IServiceBuilder builder)
    {
        builder.AddCQRSHandlers(Assembly.GetAssembly(typeof(IApplicationMarker)));
        builder.AddMediator();

        builder.AddDefaultDbContext<DefaultDbContext>(options => 
            options.UseAutoConfigurations()
                .UseDomainConventions()
                .UseSqlite("Data Source=default.db"));
        builder.AddCreateOrMigrateRule();

        builder.AddDbContext<OtherDbContext>(options =>
            options.UseAutoConfigurations()
                .UseDomainConventions()
                .UseSqlite("Data Source=other.db"));
        builder.AddCreateOrMigrateRule<OtherDbContext>();

        builder.AddAutoRepositories<DbContext>(Assembly.GetAssembly(typeof(IDomainMarker))!, 
            @namespace: "Farewell.Debug.Domain.Entities.Customers", true);
        builder.AddAutoRepositories<OtherDbContext>(Assembly.GetAssembly(typeof(IDomainMarker))!, 
            @namespace: "Farewell.Debug.Domain.Entities.Company", true);

        builder.AddLogger<ConsoleLogger>();
        
        return builder;
    }

    public static IServiceProvider UseInfrastructure(this IServiceProvider serviceProvider)
    {
        serviceProvider.UseStateRules();
        return serviceProvider;
    }
}