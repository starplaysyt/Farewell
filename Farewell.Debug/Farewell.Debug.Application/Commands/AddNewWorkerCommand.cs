using System.Net;
using Farewell.Abstractions.CQRS;
using Farewell.Abstractions.Infrastructure;
using Farewell.Debug.Domain.Entities.Company;

namespace Farewell.Debug.Application.Commands;

public record AddNewWorkerCommand(
    string Name,
    string Surname
    ) : Command;

public class AddNewWorkerCommandHandler(IQueryableRepository<WorkerEntity> repository)
    : AsyncHandler<AddNewWorkerCommand>
{
    public override async Task<OperationResult> HandleAsync(AddNewWorkerCommand command, CancellationToken cancellationToken = default)
    {
        await repository.AddAsync(new WorkerEntity()
        {
            Identifier = Guid.NewGuid().ToString(), FirstName = command.Name,
            LastName = command.Surname
        }, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return new OperationResult((int)HttpStatusCode.OK, "Success");
    }
}