namespace Farewell.Abstractions.Domain;

public struct NullableField<T> where T : notnull
{
    public T? Value { get; set; }

    public NullableField(T? value) => Value = value;
    
    public static implicit operator NullableField<T>(T? value) => new(value);
}