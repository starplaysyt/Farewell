using Farewell.Abstractions.General;
using Farewell.Abstractions.Mediator;

namespace Farewell.Debug.Application.Commands;

public record TestCommandWithoutResponse(string Test) : Command<TestCommandWithoutResponse>;

public class TestCommandWithoutResponseHandler : AsyncHandler<TestCommandWithoutResponse>
{
    public override async Task<OperationResult> HandleAsync(
        TestCommandWithoutResponse operation, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Command TestCommandWithoutResponse complete!");
        return new OperationResult(0, "");
    }
}