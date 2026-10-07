using Farewell.Abstractions.Components;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;

namespace Farewell.Abstractions.CQRS;

public class SingletonMediator(IScopeProvider provider)
{
    public async Task<OperationResult<TResult>> HandleAsync<TOperation, TResult>(
        TOperation operation,
        CancellationToken cancellationToken = default)
        where TOperation : IDTOComponent
        where TResult : IDTOComponent
    {
        using var serviceProvider = provider.CreateScope();
        var service = serviceProvider.GetRequiredService<AsyncHandler<TOperation, TResult>>();

        return await service.HandleAsync(operation,
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<OperationResult> HandleAsync<TOperation>(TOperation operation,
        CancellationToken cancellationToken = default)
        where TOperation : IDTOComponent
    {
        using var serviceProvider = provider.CreateScope();
        var service = serviceProvider.GetRequiredService<AsyncHandler<TOperation>>();

        return await service.HandleAsync(operation,
            cancellationToken).ConfigureAwait(false);
    }
    
    public OperationResult<TResult> HandleWait<TOperation, TResult>(TOperation operation)
        where TOperation : IDTOComponent
        where TResult : IDTOComponent
    {
        using var serviceProvider = provider.CreateScope();
        var service = serviceProvider.GetRequiredService<AsyncHandler<TOperation, TResult>>();
        return service.HandleAsync(operation).GetAwaiter().GetResult();
    }

    public OperationResult HandleWait<TOperation>(TOperation operation)
        where TOperation : IDTOComponent
    {
        using var serviceProvider = provider.CreateScope();
        var service = serviceProvider.GetRequiredService<AsyncHandler<TOperation>>();
        return service.HandleAsync(operation).GetAwaiter().GetResult();
    }

    public OperationResult<TResult> Handle<TOperation, TResult>(TOperation operation)
        where TOperation : IDTOComponent where TResult : IDTOComponent
    {
        using var serviceProvider = provider.CreateScope();
        var service = serviceProvider.GetRequiredService<SyncHandler<TOperation, TResult>>();
        return service.Handle(operation);
    }

    public OperationResult Handle<TOperation>(TOperation operation) where TOperation : IDTOComponent
    {
        using var serviceProvider = provider.CreateScope();
        var service = serviceProvider.GetRequiredService<SyncHandler<TOperation>>();
        return service.Handle(operation);
    }
}