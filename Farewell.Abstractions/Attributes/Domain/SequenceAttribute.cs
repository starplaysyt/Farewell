namespace Farewell.Abstractions.Attributes.Domain;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SequenceAttribute(string name) : Attribute
{
    public string Name { get; } = name;
    public string? Schema { get; set; }
    public int StartValue { get; set; } = 1;
    public int Increment { get; set; } = 1;
}