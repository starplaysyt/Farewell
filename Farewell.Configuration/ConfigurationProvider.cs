using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using Farewell.Abstractions.Configuration;
using Farewell.Configuration.Exceptions;
using Farewell.Configuration.Internal;

namespace Farewell.Configuration;

public class ConfigurationProvider : IConfigurationProvider, IDisposable
{
    private JsonElement _root;
    private volatile ConcurrentDictionary<string, object?> _cache = new();

    private readonly IConfigParser _parser;
    private readonly FileSystemWatcher? _watcher;
    private Timer? _debounceTimer;

    private static readonly ConcurrentDictionary<string, string[]> PathCache = new();

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ConfigurationProvider(IConfigParser parser, bool watchFile = false)
    {
        _parser = parser;
        _root = _parser.Parse();

        if (watchFile)
        {
            if (parser is not IWatchableConfigParser watchable)
                throw new InvalidOperationException(
                    $"Parser '{parser.GetType().Name}' does not support file watching. " +
                    $"Implement {nameof(IWatchableConfigParser)} to enable this feature."
                );

            _watcher = SetupWatcher(watchable.FilePath);
        }
    }

    public T? Get<T>(string path) where T : IParsable<T>
    {
        if (_cache.TryGetValue(path, out var cached))
            return (T?)cached;

        var element = Navigate(_root, path);
        var strValue = element.GetString()
                       ?? throw new InvalidOperationException(
                           $"Value at '{path}' is not a string-representable primitive"
                       );

        var value = T.Parse(strValue, CultureInfo.InvariantCulture);
        _cache.TryAdd(path, value);

        return value;
    }

    public T? GetSection<T>(string path) where T : class
    {
        if (_cache.TryGetValue(path, out var cached))
            return (T?)cached;

        var element = Navigate(_root, path);
        var value = element.Deserialize<T>(SerializerOptions);
        _cache.TryAdd(path, value);

        return value;
    }

    public T? GetSection<T>() where T : class
    {
        var path = ConfigSectionAttributeCache.GetPath<T>();
        return GetSection<T>(path);
    }

    public void Reload()
    {
        var newRoot = _parser.Parse();
        var newCache = new ConcurrentDictionary<string, object?>();

        _root = newRoot;
        _cache = newCache;
    }

    private JsonElement Navigate(JsonElement root, string path)
    {
        var parts = PathCache.GetOrAdd(path, static p => p.Split(':'));
        var current = root;

        return parts.Any(part => !current.TryGetProperty(part, out current))
            ? throw new ConfigKeyNotFoundException(path)
            : current;
    }

    private FileSystemWatcher SetupWatcher(string filePath)
    {
        var watcher = new FileSystemWatcher(
            Path.GetDirectoryName(filePath)!,
            Path.GetFileName(filePath))
        {
            NotifyFilter = NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        };

        watcher.Changed += (_, _) =>
        {
            _debounceTimer?.Dispose();
            _debounceTimer = new Timer(_ => Reload(), null,
                dueTime: 300,
                period: Timeout.Infinite
            );
        };
        return watcher;
    }

    public void Dispose()
    {
        _watcher?.Dispose();
        _debounceTimer?.Dispose();

        GC.SuppressFinalize(this);
    }
}