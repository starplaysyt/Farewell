using Farewell.Abstractions.Components;

namespace Farewell.Application.CQRS;

public abstract class CQRSSyncHandler<TRequest, TResult> : IService
    where TRequest : IDTOComponent
    where TResult : IDTOComponent
{
    public abstract CommandResult<TResult> Handle(TRequest command);
}