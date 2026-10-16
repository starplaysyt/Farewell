using Farewell.Abstractions.CQRS;

namespace Farewell.Debug.Application.Queries;

public record TestQuery : Query;

public record ReturnDTO;

public class TestQueryHandler : AsyncHandler<TestQuery, ReturnDTO[]>
{
    public override async Task<OperationResult<ReturnDTO[]>> HandleAsync(TestQuery command, CancellationToken cancellationToken = default)
    {
        return null;
    }
}