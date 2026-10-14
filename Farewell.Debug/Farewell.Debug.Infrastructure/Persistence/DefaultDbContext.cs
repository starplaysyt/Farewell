using Farewell.Debug.Domain.Entities;
using Farewell.Debug.Domain.Entities.Customers;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug.Infrastructure.Persistence;

public class DefaultDbContext(DbContextOptions<DefaultDbContext> options) : DbContext(options)
{
    public DbSet<AnimalEntity> Animals { get; set; }
    
    public DbSet<OwnerEntity> Owners { get; set; }
}