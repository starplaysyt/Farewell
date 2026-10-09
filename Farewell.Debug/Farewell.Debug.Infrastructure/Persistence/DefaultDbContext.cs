using Farewell.Debug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug.Infrastructure.Persistence;

public class DefaultDbContext(DbContextOptions<DefaultDbContext> options) : DbContext(options)
{
    public DbSet<AnimalEntity> Animals { get; set; }
}