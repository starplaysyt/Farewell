using Farewell.Debug.TestApplicationInterfaces;
using Farewell.Debug.TestEntities;
using Farewell.Debug.TestInfrastructureRepositories;
using Farewell.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug;

public static class RepositoryDebug
{
    public static void Run()
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
    }
}