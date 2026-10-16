using System.Reflection;
using Farewell.Abstractions.CQRS;
using Farewell.Abstractions.Extensions;
using Farewell.Debug.Application.Queries;
using Farewell.Debug.Infrastructure;
using Farewell.DI;

namespace Farewell.Debug.Presentation.Console;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = new ServiceBuilder();
        builder.AddInfrastructure();

        var provider = builder.Build();
        provider.UseInfrastructure();

        using var scope = provider.CreateScope();

        // scope.GetService<AsyncHandler<TestQuery, ReturnDTO[]>>().HandleAsync(new TestQuery());

        scope.GetService<Mediator>().HandleWait(new TestQuery());
    }
}