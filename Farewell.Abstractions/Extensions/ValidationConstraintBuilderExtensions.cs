using Farewell.Abstractions.Validation;

namespace Farewell.Abstractions.Extensions;

public static class ValidationConstraintBuilderExtensions
{
    public static IPropertyConstraintBuilder<string?> NotEmpty(
        this IPropertyConstraintBuilder<string?> b)
        => b.AddCheck(ValidationCode.NotEmpty,
            (v, _, _) => new ValueTask<bool>(!string.IsNullOrEmpty(v)));

    public static IPropertyConstraintBuilder<string?> MinLength(
        this IPropertyConstraintBuilder<string?> b, int min)
        => b.AddCheck(ValidationCode.MinLength,
            (v, _, _) => new ValueTask<bool>(v is not null && v.Length >= min));

    public static IPropertyConstraintBuilder<string?> MinLength(
        this IPropertyConstraintBuilder<string?> b, Func<IServiceProvider, int> min)
        => b.AddCheck(ValidationCode.MinLength,
            (v, sp, _) => new ValueTask<bool>(v is not null && v.Length >= min(sp)));

    public static IPropertyConstraintBuilder<string?> MaxLength(
        this IPropertyConstraintBuilder<string?> b, int max)
        => b.AddCheck(ValidationCode.MaxLength,
            (v, _, _) => new ValueTask<bool>(v is null || v.Length <= max));

    public static IPropertyConstraintBuilder<string?> MaxLength(
        this IPropertyConstraintBuilder<string?> b, Func<IServiceProvider, int> max)
        => b.AddCheck(ValidationCode.MaxLength,
            (v, sp, _) => new ValueTask<bool>(v is null || v.Length <= max(sp)));

    public static IPropertyConstraintBuilder<string?> Matches(
        this IPropertyConstraintBuilder<string?> b, string pattern)
        => b.AddCheck(ValidationCode.InvalidFormat,
            (v, _, _) => new ValueTask<bool>(
                v is null || System.Text.RegularExpressions.Regex.IsMatch(v, pattern)));

    public static IPropertyConstraintBuilder<string?> Email(
        this IPropertyConstraintBuilder<string?> b)
        => b.AddCheck(ValidationCode.InvalidEmail,
            (v, _, _) => new ValueTask<bool>(
                v is null || v.Contains('@') && v.Contains('.')));

    public static IPropertyConstraintBuilder<TValue> InRange<TValue>(
        this IPropertyConstraintBuilder<TValue> b, TValue min, TValue max)
        where TValue : struct, IComparable<TValue>
        => b.AddCheck(ValidationCode.OutOfRange,
            (v, _, _) => new ValueTask<bool>(
                v.CompareTo(min) >= 0 && v.CompareTo(max) <= 0));

    public static IPropertyConstraintBuilder<TValue> Positive<TValue>(
        this IPropertyConstraintBuilder<TValue> b)
        where TValue : struct, IComparable<TValue>
        => b.AddCheck(ValidationCode.MustBePositive,
            (v, _, _) => new ValueTask<bool>(v.CompareTo(default) > 0));

    public static IPropertyConstraintBuilder<TValue> Negative<TValue>(
        this IPropertyConstraintBuilder<TValue> b)
        where TValue : struct, IComparable<TValue>
        => b.AddCheck(ValidationCode.MustBeNegative,
            (v, _, _) => new ValueTask<bool>(v.CompareTo(default) < 0));

    public static IPropertyConstraintBuilder<int> Positive(
        this IPropertyConstraintBuilder<int> b)
        => b.AddCheck(ValidationCode.MustBePositive, v => v > 0);

    public static IPropertyConstraintBuilder<int> InRange(
        this IPropertyConstraintBuilder<int> b, int min, int max)
        => b.AddCheck(ValidationCode.OutOfRange, v => v >= min && v <= max);

    public static IPropertyConstraintBuilder<decimal> Positive(
        this IPropertyConstraintBuilder<decimal> b)
        => b.AddCheck(ValidationCode.MustBePositive, v => v > 0m);

    public static IPropertyConstraintBuilder<decimal> InRange(
        this IPropertyConstraintBuilder<decimal> b, decimal min, decimal max)
        => b.AddCheck(ValidationCode.OutOfRange, v => v >= min && v <= max);
}