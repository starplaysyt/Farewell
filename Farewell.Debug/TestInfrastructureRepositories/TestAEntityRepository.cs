using Farewell.Debug.TestApplicationInterfaces;
using Farewell.Debug.TestEntities;
using Farewell.Infrastructure;

namespace Farewell.Debug.TestInfrastructureRepositories;

public class TestAEntityRepository(TestDbContext context) 
    : EFRepository<TestAEntity, uint>(context), ITestAEntityRepository
{
}