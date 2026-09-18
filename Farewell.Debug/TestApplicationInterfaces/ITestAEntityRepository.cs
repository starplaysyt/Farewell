using Farewell.Abstractions.Infrastructure;
using Farewell.Debug.TestEntities;

namespace Farewell.Debug.TestApplicationInterfaces;

public interface ITestAEntityRepository : IQueryableRepository<TestAEntity, uint>
{
}