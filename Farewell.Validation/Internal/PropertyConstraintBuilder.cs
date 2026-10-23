using Farewell.Abstractions.General;
using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Internal;

internal sealed class PropertyConstraintBuilder<TValue>
    : IPropertyConstraintBuilder<TValue>
{
    private readonly List<CheckEntry<TValue>> _checks = [];

    public IPropertyConstraintBuilder<TValue> AddCheck(
        ResultCode code,
        Func<TValue, IServiceProvider, CancellationToken, ValueTask<bool>> check)
    {
        _checks.Add(new CheckEntry<TValue>(code, check));
        return this;
    }

    public IPropertyConstraintBuilder<TValue> AddCheck(
        ResultCode code,
        Func<TValue, IServiceProvider, bool> check)
    {
        _checks.Add(new CheckEntry<TValue>(code,
            (v, sp, _) => new ValueTask<bool>(check(v, sp))));
        return this;
    }

    public IPropertyConstraintBuilder<TValue> AddCheck(
        ResultCode code,
        Func<TValue, bool> check)
    {
        _checks.Add(new CheckEntry<TValue>(code,
            (v, _, _) => new ValueTask<bool>(check(v))));
        return this;
    }

    internal CheckEntry<TValue>[] BuildChecks() => [.. _checks];
}