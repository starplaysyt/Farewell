using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Mediator;
using Farewell.Debug.Application.Commands;
using Farewell.Debug.Application.Queries;
using Farewell.Debug.Application.RepositoryUsageTestCommands;
using Farewell.Debug.Infrastructure;
using Farewell.DI;

namespace Farewell.Debug.Presentation.Console;

public class Program
{
    public static void Main(string[] args)
    {
        // var builder = new ServiceBuilder();
        // builder.AddInfrastructure();
        //
        // var provider = builder.Build();
        // provider.UseInfrastructure();
        //
        // using var scope = provider.CreateScope();
        //
        // // scope.GetService<AsyncHandler<TestQuery, ReturnDTO[]>>().HandleAsync(new TestQuery());
        //
        // var result = scope.GetRequiredService<IMediator>().HandleWait(new TestQuery());
        // var result2 = scope.GetRequiredService<IMediator>().HandleWait(new TestCommand("Test"));
        // var result3 = scope.GetRequiredService<IMediator>().HandleWait(new TestCommandWithoutResponse("Test"));
        // var result4 = scope.GetRequiredService<IMediator>().HandleWait(new TestQueryWithoutResponse());

        var builder = new ServiceBuilder();
        builder.AddInfrastructure();
        var provider = builder.Build();
        provider.UseInfrastructure();
        
        using var scope =  provider.CreateScope();
        
        var mediator = scope.GetRequiredService<IMediator>();
        
        mediator.HandleWait(new SeedTestDataCommand());
        
        mediator.HandleAsync(new DeleteImageCommand(1, ForceDelete: false));
        
        mediator.HandleAsync(new DeleteImageCommand(2, ForceDelete: true));
        
        mediator.HandleAsync(new DeleteImageCommand(1, ForceDelete: true));
        
        mediator.HandleAsync(new DeleteImageCommand(999));
    }
}