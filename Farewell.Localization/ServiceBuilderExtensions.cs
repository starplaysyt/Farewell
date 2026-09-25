using Farewell.Abstractions.DI;
using Farewell.Abstractions.Exceptions;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Localization;
using Farewell.Localization.Internal;

namespace Farewell.Localization;

public static class LocalizationExtensions
{
    public static IServiceBuilder AddLocaleModules(
        this IServiceBuilder services,
        Action<LocalizationModulesBuilder> configure)
    {
        var builder = new LocalizationModulesBuilder();
        configure(builder);

        var modules = builder.Build();

        services.AddSingleton<ILocaleModulesProvider>((_) => new LocaleModulesProvider(modules));

        services.AddKeyedSingleton<ILocalizationProvider>(
            "default",
            sp =>
            {
                var modulesProvider = sp.GetRequiredService<ILocaleModulesProvider>();
                return new DefaultLocaleProvider(modulesProvider.Modules);
            }
        );

        services.AddSingleton<IDefaultLocaleProvider>(sp =>
            (IDefaultLocaleProvider)sp.GetRequiredKeyedService<ILocalizationProvider>(
                "default"
            )
        );

        return services;
    }

    public static IServiceBuilder AddLocalizationProvider(
        this IServiceBuilder services,
        string localeKey)
    {
        if (string.IsNullOrWhiteSpace(localeKey))
            throw new LocalizationException("Locale key cannot be null or whitespace");

        services.AddKeyedSingleton<ILocalizationProvider>(
            localeKey,
            sp =>
            {
                var defaultProvider = sp.GetRequiredService<IDefaultLocaleProvider>();
                return new LocaleProvider(defaultProvider, localeKey);
            }
        );

        return services;
    }
}