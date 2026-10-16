using Farewell.Abstractions.Mediator;

namespace Farewell.Debug.Application.Queries;

public record TestQuery : Query<TestQuery, TestQueryResponse>;

public record TestQueryResponse(string Test);

public class TestQueryHandler : AsyncHandler<TestQuery, TestQueryResponse>
{
    public override async Task<OperationResult<TestQueryResponse>> HandleAsync(TestQuery operation,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Query TestQuery complete!");
        return new OperationResult<TestQueryResponse>(0, "", new TestQueryResponse("Test"));
    }
}