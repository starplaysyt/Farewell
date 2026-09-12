namespace Farewell.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceGroup() : Attribute
{
    public string[] Tags {get; set;}
}