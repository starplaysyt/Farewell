using Farewell.Abstractions.Components;

namespace Farewell.Application.CQRS;

public abstract class CQRSSyncHandler<TRequest, TResult> : IService
    where TRequest : IDTOComponent
    where TResult : IDTOComponent
{
    public abstract OperationResult<TResult> Handle(TRequest command);
}

public abstract class CQRSSyncHandler<TRequest> : IService
    where TRequest : IDTOComponent
{
    public abstract OperationResult Handle(TRequest command);
}