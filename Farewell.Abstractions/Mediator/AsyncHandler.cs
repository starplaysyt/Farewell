using Farewell.Abstractions.General;
using Farewell.Abstractions.Validation;

namespace Farewell.Abstractions.Mediator;

public abstract class AsyncHandler<TRequest, TResult>
    where TRequest : IResultOperation<TRequest, TResult>
{
    public abstract Task<OperationResult<TResult>> HandleAsync(TRequest operation,
        CancellationToken cancellationToken = default);
}

public abstract class AsyncHandler<TRequest>
    where TRequest : IResultOperation<TRequest>
{
    public abstract Task<OperationResult> HandleAsync(TRequest operation,
        CancellationToken cancellationToken = default);
}

public abstract class ValidationAsyncHandler<TRequest>
    where TRequest : IResultOperation<TRequest, ValidationReport?>
{
    public abstract Task<OperationResult<ValidationReport?>> HandleAsync(TRequest operation,
        CancellationToken cancellationToken = default);
}