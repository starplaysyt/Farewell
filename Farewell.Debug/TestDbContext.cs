using Farewell.Debug.TestEntities;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug;

public class TestDbContext(DbContextOptionsBuilder builder) : DbContext(builder.Options)
{
    public DbSet<TestAEntity> TestAEntities { get; set; }
}