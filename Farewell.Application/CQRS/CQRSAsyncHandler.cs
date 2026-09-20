using Farewell.Abstractions.Components;

namespace Farewell.Application.CQRS;

public abstract class CQRSAsyncHandler<TRequest, TResult> : IService
    where TRequest : IDTOComponent
    where TResult : IDTOComponent
{
    public abstract Task<OperationResult<TResult>> HandleAsync(TRequest command,
        CancellationToken cancellationToken = default);
}

public abstract class CQRSAsyncHandler<TRequest> : IService
    where TRequest : IDTOComponent
{
    public abstract Task<OperationResult> HandleAsync(TRequest command,
        CancellationToken cancellationToken = default);
}