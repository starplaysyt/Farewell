namespace Farewell.Abstractions.Configuration.Exceptions;

public sealed class ConfigurationResolvingException(string path, Exception inner)
    : ConfigurationException($"Failed to resolve configuration at '{path}': {inner.Message}", inner)
{
    public string ConfigPath { get; } = path;
}