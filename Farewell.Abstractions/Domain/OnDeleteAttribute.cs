namespace Farewell.Abstractions.Domain;

[AttributeUsage(AttributeTargets.Class)]
public sealed class OnDeleteAttribute(OnDeleteBehavior behavior) : Attribute
{
    public OnDeleteBehavior Behavior { get; } = behavior;
}

public enum OnDeleteBehavior
{
    Restrict,
    Cascade,
    SetNull
}