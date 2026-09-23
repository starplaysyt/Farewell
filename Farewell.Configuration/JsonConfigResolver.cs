using System.Text.Json;
using Farewell.Abstractions.Configuration;
using Farewell.Abstractions.Configuration.Exceptions;

namespace Farewell.Configuration;

internal sealed class JsonConfigResolver : IConfigResolver
{
    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
    };

    public T Resolve<T>(string absolutePath) where T : class
    {
        using var stream = File.OpenRead(absolutePath);
        
        var result = JsonSerializer.Deserialize<T>(stream, ReadOptions);
        
        if (result is null)
            throw new ConfigurationResolvingException(
                absolutePath,
                new InvalidOperationException("Deserialization returned null."));

        return result;
    }

    public void SaveDefaults<T>(string absolutePath, T instance) where T : class
    {
        using var stream = File.Create(absolutePath);
        JsonSerializer.Serialize(stream, instance, WriteOptions);
    }
}