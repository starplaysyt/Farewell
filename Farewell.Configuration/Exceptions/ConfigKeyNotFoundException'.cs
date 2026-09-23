namespace Farewell.Configuration.Exceptions;

public class ConfigKeyNotFoundException(string path)
    : Exception($"Configuration key '{path}' was not found")
{
    public string ConfigPath { get; } = path;
}