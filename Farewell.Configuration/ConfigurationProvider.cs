using Farewell.Abstractions.Configuration;
using Farewell.Abstractions.Configuration.Exceptions;
using Farewell.Abstractions.Validation;

namespace Farewell.Configuration;

internal sealed class ConfigurationProvider<T> : IConfigurationProvider<T>
    where T : class, new()
{
    private readonly string _path;
    private readonly IConfigResolver _resolver;
    private readonly IValidationProvider? _validationProvider;
    private volatile T _cached;

    public ConfigurationProvider(string path, IConfigResolver resolver, IValidationProvider? validationProvider = null)
    {
        _path = path;
        _resolver = resolver;
        _cached = Load();
        _validationProvider = validationProvider;
    }

    public T Get() => _cached;

    public void Reload() => _cached = Load();

    private T Load()
    {
        if (!File.Exists(_path))
        {
            var defaults = new T();
            EnsureDirectoryExists(_path);
            _resolver.SaveDefaults(_path, defaults);
            return defaults;
        }

        T resolved;

        try
        {
            resolved = _resolver.Resolve<T>(_path);
        }
        catch (ConfigurationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ConfigurationResolvingException(_path, ex);
        }
        
        if (_validationProvider == null) return resolved;

        var validationReport = _validationProvider.ValidateAll(resolved);

        return validationReport.IsSuccess ? resolved : 
            throw new ConfigurationValidationException(validationReport);
    }

    private static void EnsureDirectoryExists(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
    }
}