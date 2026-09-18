using System.Runtime.CompilerServices;

namespace Farewell.Abstractions.Domain;

public readonly struct NullableField<T>(T? value)
{
    public readonly T? Value = value;
    public static implicit operator NullableField<T>(T? value) => new(value);
}