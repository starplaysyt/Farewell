

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

using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.DI;

namespace Farewell.Debug;

// public interface IResolver
// {
//     string ResolverName { get; }
// }
//
// public class JsonResolver : IResolver
// {
//     public string ResolverName => "jsonResolver";
// }
//
// public class YamlResolver : IResolver
// {
//     public string ResolverName => "yamlResolver";
// }
//
// public class ConfigurationProvider(string path)
// {
//     
// }

public class Program
{
    public static void Main(string[] args)
    {
        // ServiceBuilder serviceBuilder = new ServiceBuilder();
        // serviceBuilder.AddKeyedSingleton<ITestContext, TestImplementationA>("A");
        // serviceBuilder.AddKeyedSingleton<ITestContext, TestImplementationA>("B");
        //
        // var prov = serviceBuilder.Build();
        //
        // var result = prov.GetRequiredKeyedService<ITestContext>("B");
        //
        // Console.WriteLine(result.GetType().FullName);
        


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