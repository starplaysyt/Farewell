using Farewell.Abstractions.General;
using Farewell.Abstractions.Mediator;

namespace Farewell.Debug.Application.Queries;

public record TestQueryWithoutResponse : Query<TestQueryWithoutResponse>;

public class TestQueryWithoutResponseHandler : AsyncHandler<TestQueryWithoutResponse>
{
    public override async Task<OperationResult> HandleAsync(TestQueryWithoutResponse operation,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Query TestQueryWithoutResponse complete!");
        return new OperationResult(0, "");
    }
}