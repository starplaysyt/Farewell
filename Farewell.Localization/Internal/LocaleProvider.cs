using System.Collections.Frozen;
using System.Text.Json;
using Farewell.Abstractions.Exceptions;
using Farewell.Abstractions.Localization;

namespace Farewell.Localization.Internal;

internal sealed class LocaleProvider : ILocalizationProvider
{
    private readonly IDefaultLocaleProvider _defaultLocaleProvider;
    private readonly string _localeKey;
    private readonly string _filePath;
    private volatile FrozenDictionary<string, string> _data;

    public string this[string key] => _data.TryGetValue(key, out var value)
        ? value
        : throw new LocalizationException($"Key '{key}' not found in locale provider '{_localeKey}'");

    public LocaleProvider(IDefaultLocaleProvider defaultLocaleProvider, string localeKey)
    {
        _defaultLocaleProvider = defaultLocaleProvider;
        _localeKey = localeKey;
        _filePath = Path.Combine(AppContext.BaseDirectory, "locales", $"{localeKey}.json");
        _data = Load();
    }

    public void Reload()
    {
        var newData = Load();
        _data = newData;
    }

    private FrozenDictionary<string, string> Load()
    {
        EnsureFileExists();

        var fileDict = ReadFile();
        var needsRewrite = SynchronizeWithDefault(fileDict);

        if (needsRewrite)
            WriteFile(fileDict);

        return BuildFrozenDictionary(fileDict);
    }

    private void EnsureFileExists()
    {
        if (File.Exists(_filePath))
            return;

        var emptyDict = _defaultLocaleProvider.Data.Keys
            .ToDictionary(key => key, _ => (string?)null);

        WriteFile(emptyDict);
    }

    private Dictionary<string, string?> ReadFile()
    {
        var json = File.ReadAllText(_filePath);
        var options = new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        return JsonSerializer.Deserialize<Dictionary<string, string?>>(json, options)
            ?? throw new LocalizationException(
                $"Failed to deserialize locale file '{_filePath}'"
            );
    }

    private bool SynchronizeWithDefault(Dictionary<string, string?> fileDict)
    {
        var needsRewrite = false;
        var defaultKeys = _defaultLocaleProvider.Data.Keys.ToHashSet();
        
        foreach (var key in defaultKeys.Where(key => !fileDict.ContainsKey(key)))
        {
            fileDict[key] = null;
            needsRewrite = true;
        }
        
        foreach (var key in fileDict.Keys.Where(key => !defaultKeys.Contains(key)).ToList())
        {
            fileDict.Remove(key);
            needsRewrite = true;
        }

        return needsRewrite;
    }

    private void WriteFile(Dictionary<string, string?> fileDict)
    {
        var defaultData = _defaultLocaleProvider.Data;
        var orderedKeys = _defaultLocaleProvider.OrderedKeys;

        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);

        using var stream = File.Open(_filePath, FileMode.Create, FileAccess.Write);
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

        writer.WriteStartObject();

        foreach (var key in orderedKeys)
        {
            writer.WriteCommentValue($" Default: \"{defaultData[key]}\"");

            if (fileDict.TryGetValue(key, out var value) && value is not null)
                writer.WriteString(key, value);
            else
                writer.WriteNull(key);
        }

        writer.WriteEndObject();
    }

    private FrozenDictionary<string, string> BuildFrozenDictionary(Dictionary<string, string?> fileDict)
    {
        var defaultData = _defaultLocaleProvider.Data;

        return fileDict.ToFrozenDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value ?? defaultData[kvp.Key]
        );
    }
}