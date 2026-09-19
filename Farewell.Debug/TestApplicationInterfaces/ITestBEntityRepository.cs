using Farewell.Abstractions.Infrastructure;
using Farewell.Debug.TestEntities;

namespace Farewell.Debug.TestApplicationInterfaces;

public interface ITestBEntityRepository : IQueryableRepository<TestBEntity, uint>
{
}