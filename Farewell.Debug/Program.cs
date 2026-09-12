using System.Reflection;
using Farewell.Abstractions.Attributes;
using Farewell.Abstractions.Components;

namespace Farewell.Debug
{
    [ServiceGroup(Tags = ["FeatureGroup1", "FeatureGroup2"])]
    public class TestCommand : ICommand
    {
    
    }

    [ServiceGroup(Tags = ["FeatureGroup1", "FeatureGroup3"])]
    public class TestCommand3 : ICommand
    {
        
    }
    
    public class CommandImplementationTest : ICommand
    {
    
    }

    public class TestCommand2 : CommandImplementationTest
    {
    
    }
}

namespace Farewell.Debug.Implementation.Tests
{
    public class TestCommand : ICommand
    {
        
    }
    
    public class TestCommand4 : ICommand
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
                        && t is { IsClass: true, IsAbstract: false } && (t.Namespace ?? "").StartsWith(nameSpace))
            .ToArray(); // lookups every ICommand from selected lookupNamespace

        return result;
    }

    public static string[] GetServiceGroups<TServiceType>()
    {
        var groupsTotal = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(TServiceType).IsAssignableFrom(t))
            .SelectMany(t => t.GetCustomAttribute<ServiceGroup>()?.Tags ?? [], (_, s) => s)
            .Distinct()
            .ToArray();
        
        return groupsTotal;
    }

    public static Dictionary<string, List<Type>> GetServiceByGroups<TServiceType>(string nameSpace = "")
    {
        // var dictionary = new Dictionary<string, List<Type>>();




        // Assembly.GetExecutingAssembly().GetTypes()
        //     .Where(t => typeof(TServiceType).IsAssignableFrom(t)
        //     && typeof(TServiceType).GetCustomAttribute<ServiceGroup>() is not null)

        return null;
    }
    
    public static void Main(string[] args)
    {
        var commandType = typeof(ICommand);
        
        var assembly = Assembly.GetExecutingAssembly();

        var lookupNamespace = nameof(Farewell.Debug);

        var result = assembly.GetTypes()
            .Where(t => commandType.IsAssignableFrom(t) 
                        && t is { IsClass: true, IsAbstract: false } && t.Namespace.StartsWith(lookupNamespace))
            .ToArray(); // lookups every ICommand from selected lookupNamespace
        
        
        // Use StartsWith filter to get types from namespace
        
        foreach (var command in result)
            Console.WriteLine(command.Name);


        Console.WriteLine("=============");
        
        var allGroups = GetServiceGroups<ICommand>();
        
        foreach (var command in allGroups)
            Console.WriteLine(command);
    }
}