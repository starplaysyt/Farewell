namespace Farewell.Abstractions.Attributes.Domain;

public enum DeleteAction
{
    Cascade,
    Restrict,
    SetNull,
    SetDefault,
    NoAction
}