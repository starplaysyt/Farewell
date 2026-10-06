using Farewell.Abstractions.DI;
using Farewell.Compatibility.MS.DependencyInjection;
using Farewell.Debug.ASP.Infrastructure.Persistence;
using Farewell.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Farewell.Debug.ASP.Infrastructure;

public static class InfrastructureDIExtensions
{
    public static IServiceBuilder AddInfrastructure(this IServiceBuilder serviceBuilder)
    {
        serviceBuilder.AddDefaultDbContext<AppDbContext>();
        serviceBuilder.AddGenericRepositories();

        var serviceCollection = serviceBuilder.GetServiceCollection();

        serviceCollection.AddDbContext<AppDbContext>(builder =>
            builder.UseDomainConventions().UseAutoConfigurations()
                .UseSqlite("Data Source=database.db"));

        return serviceBuilder;
    }
}