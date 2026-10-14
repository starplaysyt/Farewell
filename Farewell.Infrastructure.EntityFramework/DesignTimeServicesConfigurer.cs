using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Farewell.Infrastructure;

public class DesignDbContextFactoryConfigurer : IDesignTimeServices
{
    public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(
            typeof(IDesignTimeDbContextFactory<>),
            typeof(DesignDbContextFactory<>));
    }
}