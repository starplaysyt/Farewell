namespace Farewell.DI;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromKeyedServicesAttribute(object key) : Attribute
{
    public object Key { get; } = key;
}