using System.Reflection;
using Farewell.Abstractions.Attributes;
using Farewell.Abstractions.Components;
using Farewell.Abstractions.Domain;
using Farewell.Abstractions.Presentation;
using Farewell.Application;
using Farewell.Application.CQRS;
using Farewell.DI;
using Farewell.Presentation;

namespace Farewell.Debug
{
    public class TestDomainEntity : DomainEntity<uint>
    {
        public string TestField1 { get; set; }
        public string TestField2 { get; set; }
        public string TestField3 { get; set; }
    }
    
    [CommandComponent]
    public record TestCommand : CQRSCommand
    {
        public required string TestData1 { get; set; }
        public required string TestData2 { get; set; }
        public required string TestData3 { get; set; }
    }

    [QueryComponent("TestGroup")]
    public record TestQuery : CQRSQuery
    {
        public required string TestData1 { get; set; }
        public required string TestData2 { get; set; }
        public required string TestData3 { get; set; }
    }

    public record TestResponse(string TestField1, string TestField2) : IDTOComponent;
    
    [HandlerComponent]
    public class TestHandler : CQRSAsyncHandler<TestQuery, TestResponse>
    {
        public override async Task<CommandResult<TestResponse>> HandleAsync(TestQuery command, CancellationToken cancellationToken = default)
        {
            return new CommandResult<TestResponse>(100, "TestMessage", new TestResponse("test1", "test2"));
        }
    }
}

namespace Farewell.Debug.Implementation.Tests
{
    public record TestCommand : CQRSCommand
    {
    }

    public record TestCommand4 : CQRSCommand
    {
    }
}


// var builder = new FarewellBuilder();
//
// builder
//      .ConfigureAutoLoadNamespaces(conf => {
//          conf.UseRecursiveSearchStrategy(); // loads not only types what loaded in this namespace, but in subnamespaces too.
//          conf.SetCommandsNamespace("Farewell.Debug.Commands");
//          conf.SetQueriesNamespace("Farewell.Debug.Queries");
//          conf.SetConfigurationNamespace("Farewell.Debug.Configuration");
//          conf.SetDTONamespace("Farewell.Debug.DTOs");
//      });
//
// builder
//      .ConfigureLocalization
//
// builder
//      .SetupServiceRegistration(bld => {
//          bld.RegisterServices<ICommand>();
//          bld.RegisterServices<IQuery>();
//          bld.RegisterServices([
//              typeof(TestCommand1),
//              typeof(TestCommand2),
//              typeof(TestCommand3),
//          ]);
//      });

// builder
//      .AddPipeline(selector => {
//          selector.SelectTargetGroup<PipelineAGroup>();
//          selector.SelectTarget<TestCommand1>();
//      },
//      pipeline => {
//          pipeline.RunMiddleware<TestMiddleware>();
//          pipeline.RunHandle();
//          pipeline.RunMiddleware<TestMiddleware2>();
//      });

// builder
//     .AddPipeline<PipelineAGroup>(pipeline => {       // or use AddPipeline<TestCommand1>, or AddPipeline<ICommand>
//          pipeline.RunMiddleware<TestPreMiddleware>();
//          pipeline.RunHandle();
//          pipeline.RunMiddleware<TestMiddleware2>();
//     });

public class Program
{
    public static Type[] GetServiceTypes<TServiceType>(string nameSpace = "")
    {
        var assembly = Assembly.GetExecutingAssembly();

        var result = assembly.GetTypes()
            .Where(t => typeof(TServiceType).IsAssignableFrom(t)
                        && t is { IsClass: true, IsAbstract: false } &&
                        (t.Namespace ?? "").StartsWith(nameSpace))
            .ToArray(); // lookups every ICommand from selected lookupNamespace

        return result;
    }

    public static string? GetServiceGroup<TServiceType>()
    {
        var groupsTotal = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(TServiceType).IsAssignableFrom(t))
            .Select(t => t.GetCustomAttribute<CommandComponent>()?.Group)
            .FirstOrDefault();

        return groupsTotal;
    }

    public static Dictionary<string, List<Type>> GetServiceByGroups<TServiceType>(
        string nameSpace = "")
    {
        // var dictionary = new Dictionary<string, List<Type>>();


        // Assembly.GetExecutingAssembly().GetTypes()
        //     .Where(t => typeof(TServiceType).IsAssignableFrom(t)
        //     && typeof(TServiceType).GetCustomAttribute<ServiceGroup>() is not null)

        return null;
    }

    public static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.AddServiceScope("Farewell.Abstractions");

        var assembly = Assembly.GetCallingAssembly();
        foreach (var type in assembly.GetTypes())
        {
            Console.WriteLine($"EXECASM: {type.Name}");
        }

        // var commandType = typeof(CQRSCommand);
        //
        // var assembly = Assembly.GetExecutingAssembly();
        //
        // var lookupNamespace = nameof(Farewell.Debug);
        //
        // var result = assembly.GetTypes()
        //     .Where(t => commandType.IsAssignableFrom(t)
        //                 && t is { IsClass: true, IsAbstract: false } &&
        //                 t.Namespace.StartsWith(lookupNamespace))
        //     .ToArray(); // lookups every ICommand from selected lookupNamespace
        //
        //
        // // Use StartsWith filter to get types from namespace
        //
        // foreach (var command in result)
        //     Console.WriteLine(command.Name);
        //
        //
        // Console.WriteLine("=============");
        //
        // var allGroups = GetServiceGroup<CQRSCommand>();
        //
        // Console.WriteLine(allGroups);
        //
        // var builder = new ConfigurationBuilder();
        // builder.AddServiceBuilder<ServiceBuilder>();
    }
}