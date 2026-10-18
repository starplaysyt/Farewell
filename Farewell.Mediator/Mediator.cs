using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.General;
using Farewell.Abstractions.Mediator;

namespace Farewell.Mediator;

public class Mediator(IServiceProvider provider) : IMediator
{
    public async Task<OperationResult<TResult>> HandleAsync<TRequest, TResult>(IResultOperation<TRequest, TResult>
        operation,
        CancellationToken cancellationToken = default)
        where TRequest : IResultOperation<TRequest, TResult>
    {
        var service = provider.GetRequiredService<AsyncHandler<TRequest, TResult>>();

        return await service.HandleAsync((TRequest)operation,
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<OperationResult> HandleAsync<TRequest>(IResultOperation<TRequest> operation,
        CancellationToken cancellationToken = default)
        where TRequest : IResultOperation<TRequest>
    {
        var service = provider.GetRequiredService<AsyncHandler<TRequest>>();

        return await service.HandleAsync((TRequest)operation,
            cancellationToken).ConfigureAwait(false);
    }
    
    public OperationResult<TResult> HandleWait<TRequest, TResult>(IResultOperation<TRequest, TResult> operation)
        where TRequest : IResultOperation<TRequest, TResult>
    {
        var service = provider.GetRequiredService<AsyncHandler<TRequest, TResult>>();
        return service.HandleAsync((TRequest)operation).GetAwaiter().GetResult();
    }

    public OperationResult HandleWait<TRequest>(IResultOperation<TRequest> operation)
        where TRequest : IResultOperation<TRequest>
    {
        var service = provider.GetRequiredService<AsyncHandler<TRequest>>();
        return service.HandleAsync((TRequest)operation).GetAwaiter().GetResult();
    }
}