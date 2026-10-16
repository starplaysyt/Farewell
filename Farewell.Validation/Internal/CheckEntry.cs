using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Internal;

internal readonly struct CheckEntry<TValue>(
    ValidationCode code,
    Func<TValue, IServiceProvider, CancellationToken, ValueTask<bool>> check)
{
    public ValidationCode Code { get; } = code;

    public Func<TValue, IServiceProvider, CancellationToken, ValueTask<bool>> Check { get; } =
        check;
}