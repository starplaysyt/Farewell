namespace Farewell.Abstractions.Attributes.Application;

[AttributeUsage(AttributeTargets.Class)]
public class GroupAttribute(string groupIdentifier) : Attribute
{
    public string GroupIdentifier { get; } = groupIdentifier;
}