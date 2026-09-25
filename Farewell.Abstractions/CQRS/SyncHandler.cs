using Farewell.Abstractions.Components;

namespace Farewell.Abstractions.CQRS;

public abstract class SyncHandler<TRequest, TResult>
    where TRequest : IDTOComponent
    where TResult : IDTOComponent
{
    public abstract OperationResult<TResult> Handle(TRequest command);
}

public abstract class SyncHandler<TRequest>
    where TRequest : IDTOComponent
{
    public abstract OperationResult Handle(TRequest command);
}