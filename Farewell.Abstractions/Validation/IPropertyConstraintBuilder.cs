using Farewell.Abstractions.General;

namespace Farewell.Abstractions.Validation;

public interface IPropertyConstraintBuilder<TValue>
{
    IPropertyConstraintBuilder<TValue> AddCheck(
        ResultCode code,
        Func<TValue, IServiceProvider, CancellationToken, ValueTask<bool>> check);

    IPropertyConstraintBuilder<TValue> AddCheck(
        ResultCode code,
        Func<TValue, IServiceProvider, bool> check);

    IPropertyConstraintBuilder<TValue> AddCheck(
        ResultCode code,
        Func<TValue, bool> check);
}