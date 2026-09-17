using Farewell.Abstractions.Components;

namespace Farewell.Application.CQRS;

public abstract class CQRSAsyncHandler<TRequest, TResult> : IService
    where TRequest : IDTOComponent
    where TResult : IDTOComponent
{
    public abstract Task<CommandResult<TResult>> HandleAsync(TRequest command,
        CancellationToken cancellationToken = default);
}