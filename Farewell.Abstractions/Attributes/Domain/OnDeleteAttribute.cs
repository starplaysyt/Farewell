namespace Farewell.Abstractions.Attributes.Domain;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class OnDeleteAttribute(DeleteAction action, string? navigation = null) : Attribute
{
    public string? Navigation { get; } = navigation;
    public DeleteAction Action { get; } = action;
}