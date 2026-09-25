using Farewell.Abstractions.Components;

namespace Farewell.Abstractions.CQRS;

public abstract class AsyncHandler<TRequest, TResult>
    where TRequest : IDTOComponent
    where TResult : IDTOComponent
{
    public abstract Task<OperationResult<TResult>> HandleAsync(TRequest command,
        CancellationToken cancellationToken = default);
}

public abstract class AsyncHandler<TRequest>
    where TRequest : IDTOComponent
{
    public abstract Task<OperationResult> HandleAsync(TRequest command,
        CancellationToken cancellationToken = default);
}