using Farewell.Validation.Constraints;

namespace Farewell.Validation.Extensions;

public static class PropertyRuleBuilderExtensions
{
    public static PropertyRuleBuilder<T, string?> NotEmpty<T>(
        this PropertyRuleBuilder<T, string?> b)
        => b.Must(new StringConstraints.NotEmptyConstraint());

    public static PropertyRuleBuilder<T, string?> MinLength<T>(
        this PropertyRuleBuilder<T, string?> b, int min)
        => b.Must(new StringConstraints.MinLengthConstraint(min));

    public static PropertyRuleBuilder<T, string?> MaxLength<T>(
        this PropertyRuleBuilder<T, string?> b, int max)
        => b.Must(new StringConstraints.MaxLengthConstraint(max));

    public static PropertyRuleBuilder<T, string?> Length<T>(
        this PropertyRuleBuilder<T, string?> b, int min, int max)
        => b.MinLength(min).MaxLength(max);

    public static PropertyRuleBuilder<T, string?> Matches<T>(
        this PropertyRuleBuilder<T, string?> b, string pattern)
        => b.Must(new StringConstraints.MatchesConstraint(pattern));

    public static PropertyRuleBuilder<T, string?> MinLength<T>(
        this PropertyRuleBuilder<T, string?> b, Func<IServiceProvider, int> min)
        => b.Must(new StringConstraints.MinLengthConstraint(min));

    public static PropertyRuleBuilder<T, string?> MaxLength<T>(
        this PropertyRuleBuilder<T, string?> b, Func<IServiceProvider, int> max)
        => b.Must(new StringConstraints.MaxLengthConstraint(max));
    
    public static PropertyRuleBuilder<T, TValue> InRange<T, TValue>(
        this PropertyRuleBuilder<T, TValue> b, TValue min, TValue max)
        where TValue : IComparable<TValue>
        => b.Must(new NumericConstraints.OutOfRangeConstraint<TValue>(min, max));

    public static PropertyRuleBuilder<T, TValue> InRange<T, TValue>(
        this PropertyRuleBuilder<T, TValue> b,
        Func<IServiceProvider, TValue> min,
        Func<IServiceProvider, TValue> max)
        where TValue : IComparable<TValue>
        => b.Must(new NumericConstraints.OutOfRangeConstraint<TValue>(min, max));
}