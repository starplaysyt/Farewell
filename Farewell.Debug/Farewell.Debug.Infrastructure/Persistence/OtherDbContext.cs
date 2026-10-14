using Farewell.Debug.Domain.Entities.Company;
using Farewell.Debug.Domain.Entities.Customers;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug.Infrastructure.Persistence;

public class OtherDbContext(DbContextOptions<OtherDbContext> options)  : DbContext(options)
{
    public DbSet<WorkerEntity> Workers { get; set; }
}