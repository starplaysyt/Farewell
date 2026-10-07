namespace Farewell.Abstractions.Attributes.Validation;

/// <summary>
/// Defines context of current validator, ex. "Creation" or "Update".
/// It is proposed to move context names to const strings.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ValidationContextAttribute : Attribute
{
    public string Context { get; }
    public ValidationContextAttribute(string context) => Context = context;
}