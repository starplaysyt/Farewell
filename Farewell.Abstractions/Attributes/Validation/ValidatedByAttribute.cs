namespace Farewell.Abstractions.Attributes.Validation;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public class ValidatedByAttribute(Type validatorType, string context = "Default") : Attribute
{
    public Type ValidatorType { get; } = validatorType;
    public string Context { get; } = context;
}