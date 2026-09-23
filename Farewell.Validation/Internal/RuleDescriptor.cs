namespace Farewell.Validation.Internal;

// Rule description for one property
// Used by Builder, compiles in one time
internal sealed class RuleDescriptor<T, TValue>
{
    public required string PropertyName { get; init; }
    public required Func<T, TValue> Selector { get; init; }
    public required ConstraintEntry<TValue>[] Constraints { get; init; }
    public Func<T, bool>? When { get; init; }
    public Func<T, bool>? Unless { get; init; }
}