namespace Farewell.Abstractions.Configuration.Exceptions;

public abstract class ConfigurationException(string message, Exception? inner = null)
    : Exception(message, inner);