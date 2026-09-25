using Farewell.Abstractions.Components;
using Farewell.Abstractions.Extensions;

namespace Farewell.Abstractions.CQRS;

public class Mediator(IServiceProvider serviceProvider)
{
    public async Task<OperationResult<TResult>> HandleAsync<TOperation, TResult>(
        TOperation operation,
        CancellationToken cancellationToken = default)
        where TOperation : IDTOComponent
        where TResult : IDTOComponent
    {
        var service = serviceProvider.GetRequiredService<AsyncHandler<TOperation, TResult>>();

        return await service.HandleAsync(operation,
            cancellationToken);
    }

    public async Task<OperationResult> HandleAsync<TOperation>(TOperation operation,
        CancellationToken cancellationToken = default)
        where TOperation : IDTOComponent
    {
        var service = serviceProvider.GetRequiredService<AsyncHandler<TOperation>>();

        return await service.HandleAsync(operation,
            cancellationToken);
    }
    
    public OperationResult<TResult> HandleWait<TOperation, TResult>(TOperation operation)
        where TOperation : IDTOComponent
        where TResult : IDTOComponent
    {
        var service = serviceProvider.GetRequiredService<AsyncHandler<TOperation, TResult>>();
        return service.HandleAsync(operation).GetAwaiter().GetResult();
    }

    public OperationResult HandleWait<TOperation>(TOperation operation)
        where TOperation : IDTOComponent
    {
        var service = serviceProvider.GetRequiredService<AsyncHandler<TOperation>>();
        return service.HandleAsync(operation).GetAwaiter().GetResult();
    }

    public OperationResult<TResult> Handle<TOperation, TResult>(TOperation operation)
        where TOperation : IDTOComponent where TResult : IDTOComponent
    {
        var service = serviceProvider.GetRequiredService<SyncHandler<TOperation, TResult>>();
        return service.Handle(operation);
    }

    public OperationResult Handle<TOperation>(TOperation operation) where TOperation : IDTOComponent
    {
        var service = serviceProvider.GetRequiredService<SyncHandler<TOperation>>();
        return service.Handle(operation);
    }
}