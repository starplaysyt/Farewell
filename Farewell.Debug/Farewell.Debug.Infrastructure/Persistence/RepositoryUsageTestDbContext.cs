using Farewell.Debug.Domain.Entities.RepositoryUsageTestEntities;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug.Infrastructure.Persistence;

public class RepositoryUsageTestDbContext(DbContextOptions<RepositoryUsageTestDbContext> options) : DbContext(options)
{
    public DbSet<TestImageEntity> TestImages { get; set; }
    public DbSet<ArticleEntity> Articles { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
}