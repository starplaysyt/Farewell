namespace Farewell.Abstractions.Attributes.Validation;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ValidationContextAttribute(string context) : Attribute
{
    public string Context { get; } = context;
}