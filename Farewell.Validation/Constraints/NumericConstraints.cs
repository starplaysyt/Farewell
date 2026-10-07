using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Constraints;

public static class NumericConstraints
{
    public sealed class OutOfRangeConstraint<TValue>(
        Func<IServiceProvider, TValue> min,
        Func<IServiceProvider, TValue> max)
        : IValidationConstraint<TValue>
        where TValue : IComparable<TValue>
    {
        public ValidationCode Code => ValidationCode.OutOfRange;

        public OutOfRangeConstraint(TValue min, TValue max)
            : this(_ => min, _ => max) { }

        public ValueTask<bool> CheckAsync(TValue value, IServiceProvider sp, CancellationToken _)
        {
            var min1 = min(sp);
            var max1 = max(sp);
            return new ValueTask<bool>(value.CompareTo(min1) >= 0 && value.CompareTo(max1) <= 0);
        }
    }
}