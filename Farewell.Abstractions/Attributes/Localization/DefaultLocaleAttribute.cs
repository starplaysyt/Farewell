namespace Farewell.Abstractions.Attributes.Localization;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class DefaultLocaleAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}