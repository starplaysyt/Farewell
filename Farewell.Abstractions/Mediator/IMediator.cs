using Farewell.Abstractions.General;

namespace Farewell.Abstractions.Mediator;

public interface IMediator
{
    public Task<OperationResult<TResult>> HandleAsync<TRequest, TResult>(
        IResultOperation<TRequest, TResult>
            operation,
        CancellationToken cancellationToken = default)
        where TRequest : IResultOperation<TRequest, TResult>;

    public Task<OperationResult> HandleAsync<TRequest>(IResultOperation<TRequest> operation,
        CancellationToken cancellationToken = default)
        where TRequest : IResultOperation<TRequest>;

    public OperationResult<TResult> HandleWait<TRequest, TResult>(
        IResultOperation<TRequest, TResult> operation)
        where TRequest : IResultOperation<TRequest, TResult>;

    public OperationResult HandleWait<TRequest>(IResultOperation<TRequest> operation)
        where TRequest : IResultOperation<TRequest>;
}