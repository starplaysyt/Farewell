using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Extensions;

public static class ValidationServiceBuilderExtensions
{
    public static IServiceBuilder AddValidation(this IServiceBuilder services)
    {
        services.AddScoped<IValidationProvider, ValidationProvider>();
        return services;
    }
}