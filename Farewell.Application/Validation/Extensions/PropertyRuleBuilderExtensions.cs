using Farewell.Application.Validation.Constraints;

namespace Farewell.Application.Validation.Extensions;

public static class PropertyRuleBuilderExtensions
{
    extension<T>(PropertyRuleBuilder<T, string?> builder)
    {
        public PropertyRuleBuilder<T, string?> NotEmpty()
            => builder.Must(new StringConstraints.NotEmptyConstraint());

        public PropertyRuleBuilder<T, string?> MinLength(int min)
            => builder.Must(new StringConstraints.MinLengthConstraint(min));

        public PropertyRuleBuilder<T, string?> MaxLength(int max)
            => builder.Must(new StringConstraints.MaxLengthConstraint(max));

        public PropertyRuleBuilder<T, string?> Length(int min, int max)
            => builder.MinLength(min).MaxLength(max);

        public PropertyRuleBuilder<T, string?> Matches(string pattern)
            => builder.Must(new StringConstraints.InvalidFormatConstraint(pattern));
    }

    public static PropertyRuleBuilder<T, TValue> InRange<T, TValue>(
        this PropertyRuleBuilder<T, TValue> builder, TValue min, TValue max)
        where TValue : IComparable<TValue>
        => builder.Must(new NumericConstraints.OutOfRangeConstraint<TValue>(min, max));
}