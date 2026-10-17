using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Mapping;

namespace Farewell.Mapping;

public static class ServiceBuilderExtensions
{
    public static IServiceBuilder AddMapping(this IServiceBuilder builder)
    {
        builder.AddScoped<IMappingProvider, MappingProvider>();
        return builder;
    }
}