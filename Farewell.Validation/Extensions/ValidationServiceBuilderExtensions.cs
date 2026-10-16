using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Validation;
using Farewell.Validation.Resolvers;

namespace Farewell.Validation.Extensions;

public static class ValidationServiceBuilderExtensions
{
    public static IServiceBuilder AddValidation(this IServiceBuilder services)
    {
        services.AddScoped<IValidationProvider, ValidationProvider>();
        return services;
    }

    public static IServiceBuilder AddValidationMessageResolver<TMessageResolver>(
        this IServiceBuilder services) where TMessageResolver : class, IValidationMessageResolver
    {
        services.AddSingleton<IValidationMessageResolver, TMessageResolver>();
        return services;
    }

    public static IServiceBuilder AddDefaultValidationMessageResolver(
        this IServiceBuilder services)
    {
        services.AddSingleton<IValidationMessageResolver, DefaultValidationMessageResolver>();
        return services;
    }

    public static IServiceBuilder AddValidationMessagePresenter<TMessagePresenter>(
        this IServiceBuilder services) where TMessagePresenter : class, IValidationPresenter
    {
        services.AddSingleton<IValidationPresenter, TMessagePresenter>();
        return services;
    }

    public static IServiceBuilder AddDefaultValidationMessagePresenter(
        this IServiceBuilder services)
    {
        services.AddSingleton<IValidationPresenter, ValidationPresenter>();
        return services;
    }
}