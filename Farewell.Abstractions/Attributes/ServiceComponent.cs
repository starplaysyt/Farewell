using Farewell.Abstractions.Components;

namespace Farewell.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceComponent(string? group = null) : Attribute, IGroupProvidable
{
    public string? Group => group;
}