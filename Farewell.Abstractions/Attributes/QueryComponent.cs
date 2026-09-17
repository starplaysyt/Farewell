using Farewell.Abstractions.Components;

namespace Farewell.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class QueryComponent(string? group = null) : Attribute, IGroupProvidable
{
    public string? Group => group;
}