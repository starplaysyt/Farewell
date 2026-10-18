using Farewell.Abstractions.General;
using Farewell.Abstractions.Mediator;

namespace Farewell.Debug.Application.Commands;

public record TestCommand(string Test) : Command<TestCommand, TestCommandResponse>;

public record TestCommandResponse(string Test);

public class TestCommandHandler : AsyncHandler<TestCommand, TestCommandResponse>
{
    public override async Task<OperationResult<TestCommandResponse>> HandleAsync(
        TestCommand operation, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Command TestCommand complete! Given Test: {operation.Test}");
        return new OperationResult<TestCommandResponse>(0, "",
            new TestCommandResponse(operation.Test));
    }
}