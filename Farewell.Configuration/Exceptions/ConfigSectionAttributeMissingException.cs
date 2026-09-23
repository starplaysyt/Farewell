namespace Farewell.Configuration.Exceptions;

public class ConfigSectionAttributeMissingException(Type type)
    : Exception($"Type '{type.Name}' is missing [ConfigSection] attribute")
{
    public Type TargetType { get; } = type;
}