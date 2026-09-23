namespace Farewell.Abstractions.Attributes.Configuration;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ConfigSectionAttribute(string path) : Attribute
{
    public string Path { get; } = path;
}