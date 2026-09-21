using Farewell.Application.Validation.Constraints;

namespace Farewell.Application.Validation.Extensions;

public static class ConstraintGroupBuilderExtensions
{
    extension<T>(ConstraintGroupBuilder<T, string?> builder)
    {
        public ConstraintGroupBuilder<T, string?> NotEmpty()
            => builder.Must(new StringConstraints.NotEmptyConstraint());

        public ConstraintGroupBuilder<T, string?> MinLength(int min)
            => builder.Must(new StringConstraints.MinLengthConstraint(min));

        public ConstraintGroupBuilder<T, string?> MaxLength(int max)
            => builder.Must(new StringConstraints.MaxLengthConstraint(max));

        public ConstraintGroupBuilder<T, string?> Length(int min, int max)
            => builder.MinLength(min).MaxLength(max);

        public ConstraintGroupBuilder<T, string?> Matches(string pattern)
            => builder.Must(new StringConstraints.InvalidFormatConstraint(pattern));
    }

    public static ConstraintGroupBuilder<T, TValue> InRange<T, TValue>(
        this ConstraintGroupBuilder<T, TValue> builder, TValue min, TValue max)
        where TValue : IComparable<TValue>
        => builder.Must(new NumericConstraints.OutOfRangeConstraint<TValue>(min, max));
}