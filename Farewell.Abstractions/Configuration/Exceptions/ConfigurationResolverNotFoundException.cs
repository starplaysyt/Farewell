namespace Farewell.Abstractions.Configuration.Exceptions;

public sealed class ConfigurationResolverNotFoundException(string extension)
    : ConfigurationException($"No resolver registered for '{extension}'. " +
                             $"Register one via AddConfigResolver<TResolver>(\"{extension}\")")
{
    public string FileExtension { get; } = extension;
}