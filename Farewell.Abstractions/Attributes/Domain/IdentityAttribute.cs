namespace Farewell.Abstractions.Attributes.Domain;

[AttributeUsage(AttributeTargets.Property)]
public sealed class IdentityAttribute(bool autoGenerate = false) : Attribute
{
    public bool AutoGenerate { get; } = autoGenerate;
}