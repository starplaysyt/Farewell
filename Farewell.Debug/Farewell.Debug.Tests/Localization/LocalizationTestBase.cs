using System.Text.Json;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Localization;
using Farewell.DI;
using Farewell.Localization;

namespace Farewell.Debug.Tests.Localization;

public abstract class LocalizationTestBase : IDisposable
{
    protected readonly string LocalesDirectory;

    protected LocalizationTestBase()
    {
        LocalesDirectory = Path.Combine(AppContext.BaseDirectory, "locales");
        Directory.CreateDirectory(LocalesDirectory);
    }

    protected string GetLocalePath(string localeKey) =>
        Path.Combine(LocalesDirectory, $"{localeKey}.json");

    protected void WriteLocaleFile(string localeKey, Dictionary<string, string?> data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(GetLocalePath(localeKey), json);
    }

    protected string ReadLocaleFileRaw(string localeKey) =>
        File.ReadAllText(GetLocalePath(localeKey));

    protected Dictionary<string, string?> ReadLocaleFileAsDict(string localeKey)
    {
        var json = ReadLocaleFileRaw(localeKey);
        return JsonSerializer.Deserialize<Dictionary<string, string?>>(
            json,
            new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip }
        )!;
    }

    protected IServiceProvider BuildServiceProvider(
        Action<LocalizationModulesBuilder> modules,
        params string[] localeKeys)
    {
        var services = new ServiceBuilder();
        services.AddLocaleModules(modules);

        foreach (var key in localeKeys)
            services.AddLocalizationProvider(key);

        return services.Build();
    }

    protected ILocalizationProvider GetProvider(IServiceProvider sp, string localeKey) =>
        sp.GetRequiredKeyedService<ILocalizationProvider>(localeKey);

    public void Dispose()
    {
        if (Directory.Exists(LocalesDirectory))
            Directory.Delete(LocalesDirectory, recursive: true);
    }
}