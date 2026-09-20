using Farewell.Abstractions.Components;
using Farewell.Abstractions.DI;

namespace Farewell.Application.CQRS;

public class CQRSMediator(IServiceProvider serviceProvider)
{
    public async Task<OperationResult<TResult>> HandleAsync<TOperation, TResult>(
        TOperation operation,
        CancellationToken cancellationToken = default)
        where TOperation : IDTOComponent
        where TResult : IDTOComponent
    {
        var service = serviceProvider.GetRequiredService<CQRSAsyncHandler<TOperation, TResult>>();

        return await service.HandleAsync(operation,
            cancellationToken);
    }

    public async Task<OperationResult> HandleAsync<TOperation>(TOperation operation,
        CancellationToken cancellationToken = default)
        where TOperation : IDTOComponent
    {
        var service = serviceProvider.GetRequiredService<CQRSAsyncHandler<TOperation>>();

        return await service.HandleAsync(operation,
            cancellationToken);
    }
    
    public OperationResult<TResult> HandleWait<TOperation, TResult>(TOperation operation)
        where TOperation : IDTOComponent
        where TResult : IDTOComponent
    {
        var service = serviceProvider.GetRequiredService<CQRSAsyncHandler<TOperation, TResult>>();
        return service.HandleAsync(operation).GetAwaiter().GetResult();
    }

    public OperationResult HandleWait<TOperation>(TOperation operation)
        where TOperation : IDTOComponent
    {
        var service = serviceProvider.GetRequiredService<CQRSAsyncHandler<TOperation>>();
        return service.HandleAsync(operation).GetAwaiter().GetResult();
    }

    public OperationResult<TResult> Handle<TOperation, TResult>(TOperation operation)
        where TOperation : IDTOComponent where TResult : IDTOComponent
    {
        var service = serviceProvider.GetRequiredService<CQRSSyncHandler<TOperation, TResult>>();
        return service.Handle(operation);
    }

    public OperationResult Handle<TOperation>(TOperation operation) where TOperation : IDTOComponent
    {
        var service = serviceProvider.GetRequiredService<CQRSSyncHandler<TOperation>>();
        return service.Handle(operation);
    }
}