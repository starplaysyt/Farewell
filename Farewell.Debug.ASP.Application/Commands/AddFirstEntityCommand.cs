using Farewell.Abstractions.CQRS;
using Farewell.Abstractions.Infrastructure;
using Farewell.Debug.ASP.Domain.Entities;

namespace Farewell.Debug.ASP.Application.Commands;

public record AddFirstEntityCommand : Command
{
    public string? Field1 { get; init; }
    public string? Field2 { get; init; }
}

public class AddFirstEntityHandler(IQueryableRepository<FirstDomainEntity> repo) : AsyncHandler<AddFirstEntityCommand>
{
    public override async Task<OperationResult> HandleAsync(AddFirstEntityCommand command, CancellationToken cancellationToken = default)
    {
        await repo.AddAsync(new FirstDomainEntity()
            { Field1 = command.Field1 ?? "zeroData", Field2 = command.Field2 ?? "zeroData" }, cancellationToken);

        return new OperationResult(200, "Success");
    }
}