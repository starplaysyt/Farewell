using Farewell.Debug.TestEntities;
using Farewell.Infrastructure;
using Farewell.Infrastructure.Conventions;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug;

public class TestDbContext(DbContextOptionsBuilder builder) : DbContext(builder.Options)
{
    public DbSet<TestAEntity> TestAEntities { get; set; }
    public DbSet<TestBEntity> TestBEntities { get; set; }
    public DbSet<TestCEntity> TestCEntities { get; set; }
    public DbSet<TestDEntity> TestDEntities { get; set; }
}