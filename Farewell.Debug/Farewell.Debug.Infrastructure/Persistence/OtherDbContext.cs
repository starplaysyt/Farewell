using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug.Infrastructure.Persistence;

public class OtherDbContext(DbContextOptions<OtherDbContext> options)  : DbContext
{
    
}