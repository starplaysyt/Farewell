using Farewell.Debug.TestApplicationInterfaces;
using Farewell.Debug.TestEntities;
using Farewell.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug.TestInfrastructureRepositories;

public class TestBEntityRepository(DbContext context) : EFRepository<TestBEntity, uint>(context), ITestBEntityRepository
{
    
}