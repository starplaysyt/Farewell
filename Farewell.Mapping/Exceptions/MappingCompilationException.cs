namespace Farewell.Mapping.Exceptions;

public class MappingCompilationException(Type sourceType, Type targetType, string details)
    : Exception($"Mapping {sourceType.Name} -> {targetType.Name}: {details}")
{
    public Type SourceType { get; } = sourceType;
    public Type TargetType { get; } = targetType;
    public string Details { get; } = details;
}