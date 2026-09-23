using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Constraints;

/// <summary>
/// Numeric constraints — для любого IComparable[T].
/// </summary>
public static class NumericConstraints
{
    public sealed class OutOfRangeConstraint<TValue> : IValidationConstraint<TValue>
        where TValue : IComparable<TValue>
    {
        private readonly TValue _min;
        private readonly TValue _max;
        public ValidationCode Code => ValidationCode.OutOfRange;

        public OutOfRangeConstraint(TValue min, TValue max)
        {
            _min = min;
            _max = max;
        }

        public bool Check(TValue value)
            => value.CompareTo(_min) >= 0 && value.CompareTo(_max) <= 0;
    }
}