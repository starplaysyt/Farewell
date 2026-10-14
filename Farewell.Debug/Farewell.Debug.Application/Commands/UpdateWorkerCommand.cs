using System.Net;
using Farewell.Abstractions.CQRS;
using Farewell.Abstractions.Infrastructure;
using Farewell.Debug.Domain.Entities.Company;

namespace Farewell.Debug.Application.Commands;

public record UpdateWorkerCommand(uint Id, WorkerEntityUpdateMap Map) : Command;

public class UpdateWorkerCommandHandler(IQueryableRepository<WorkerEntity> repository)
    : AsyncHandler<UpdateWorkerCommand>
{
    public override async Task<OperationResult> HandleAsync(UpdateWorkerCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await repository.ExecuteUpdateAsync(repository.GetQuery().Where(e => e.Id == command.Id),
            command.Map, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        
        return result < 1 
            ? new OperationResult((int)HttpStatusCode.NotFound, "Not found") 
            : new OperationResult((int)HttpStatusCode.OK, "Success");
    }
}