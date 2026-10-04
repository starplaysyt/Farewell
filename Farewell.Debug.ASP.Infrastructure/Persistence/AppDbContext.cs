using Farewell.Debug.ASP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug.ASP.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<FirstDomainEntity> FirstDomainEntities { get; set; }
}