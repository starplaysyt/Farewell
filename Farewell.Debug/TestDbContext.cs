using Farewell.Debug.TestEntities;
using Farewell.Infrastructure;
using Farewell.Infrastructure.Conventions;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug;

public class TestDbContext(DbContextOptionsBuilder builder) : DbContext(builder.Options)
{
    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions
            .Add(_ => new DomainAttributeConvention());
    }
    
    public DbSet<TestAEntity> TestAEntities { get; set; }
}