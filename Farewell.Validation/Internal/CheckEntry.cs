using Farewell.Abstractions.General;

namespace Farewell.Validation.Internal;

internal readonly struct CheckEntry<TValue>(
    ResultCode code,
    Func<TValue, IServiceProvider, CancellationToken, ValueTask<bool>> check)
{
    public ResultCode Code { get; } = code;

    public Func<TValue, IServiceProvider, CancellationToken, ValueTask<bool>> Check { get; } =
        check;
}