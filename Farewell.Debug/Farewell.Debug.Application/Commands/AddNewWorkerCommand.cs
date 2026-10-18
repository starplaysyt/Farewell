using System.Net;
using Farewell.Abstractions.General;
using Farewell.Abstractions.Mediator;
using Farewell.Abstractions.Infrastructure;
using Farewell.Debug.Domain.Entities.Company;

namespace Farewell.Debug.Application.Commands;

public record AddNewWorkerCommand(
    string Name,
    string Surname
    ) : Command<AddNewWorkerCommand>;

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