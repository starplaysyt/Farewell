namespace Farewell.Abstractions.Domain;

public struct NullableField<T>(T? value) where T : notnull
{
    public T? Value { get; set; } = value;
    public static implicit operator NullableField<T>(T? value) => new(value);
}