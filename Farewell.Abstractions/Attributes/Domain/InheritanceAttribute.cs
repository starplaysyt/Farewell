namespace Farewell.Abstractions.Attributes.Domain;

[AttributeUsage(AttributeTargets.Class)]
public sealed class InheritanceAttribute(InheritanceStrategy strategy) : Attribute
{
    public InheritanceStrategy Strategy { get; } = strategy;
}