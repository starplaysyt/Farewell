using Farewell.Debug;
using Farewell.Debug.TestApplicationInterfaces;
using Farewell.Debug.TestEntities;
using Farewell.Debug.TestInfrastructureRepositories;
using Farewell.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

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
    public static void Main(string[] args)
    {
        var testDbContext = new TestDbContext(new DbContextOptionsBuilder()
            .UseSqlite("Data Source=test.db")
            .UseDomainConventions()
            );

        if (testDbContext.Database.EnsureCreated())
        {
            List<TestAEntity> entities = new List<TestAEntity>();

            for (int i = 0; i < 100; i++)
            {
                entities.Add(new TestAEntity()
                {
                    TestNotNullableField = Guid.NewGuid().ToString(),
                    TestKey = "TestValue",
                    TestNotUpdatableField = Guid.NewGuid().ToString(),
                    TestNullableField = new Random().Next(),
                    TestStringNullableField = Guid.NewGuid().ToString()
                });
            }

            List<TestBEntity> bEntities = new List<TestBEntity>();

            for (int i = 0; i < 100; i++)
            {
                bEntities.Add(new TestBEntity()
                {
                    FieldB1 = Guid.NewGuid().ToString(),
                    FieldB2 = Guid.NewGuid().ToString(),
                    FieldB3 = Guid.NewGuid().ToString(),
                    FieldC1 = Guid.NewGuid().ToString(),
                    FieldC2 = Guid.NewGuid().ToString(),
                    FieldC3 = Guid.NewGuid().ToString(),
                });        
            }
            
            List<TestDEntity> dEntities = new List<TestDEntity>();

            for (int i = 0; i < 100; i++)
            {
                dEntities.Add(new TestDEntity
                {
                    FieldD1 = Guid.NewGuid()
                        .ToString(),
                    FieldD2 = Guid.NewGuid()
                        .ToString(),
                    FieldD3 = Guid.NewGuid()
                        .ToString(),
                    FieldC1 = Guid.NewGuid()
                        .ToString(),
                    FieldC2 = Guid.NewGuid()
                        .ToString(),
                    FieldC3 = Guid.NewGuid()
                        .ToString(),
                });        
            }
            
            testDbContext.TestAEntities.AddRange(entities);
            testDbContext.SaveChanges();
            
            testDbContext.TestBEntities.AddRange(bEntities);
            testDbContext.SaveChanges();
            
            testDbContext.TestDEntities.AddRange(dEntities);
            testDbContext.SaveChanges();
        }

        ITestAEntityRepository repo = new TestAEntityRepository(testDbContext);

        // repo.ExecuteUpdateAsync(repo.GetQuery().Where(e => e.Id % 2 == 0),
        //     new TestAEntityUpdateMap(null, new NullableField<int?>(null),
        //         new NullableField<string?>(null), null)).Wait();
        
        repo.ExecuteUpdateAsync(repo.GetQuery().Where(e => e.Id == 26),
            new TestAEntityUpdateMap(null, 10,
                null, null)).Wait();

        repo.SaveChangesAsync().Wait();
        
        testDbContext.Dispose();

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