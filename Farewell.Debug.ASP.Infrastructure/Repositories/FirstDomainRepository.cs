using Farewell.Debug.ASP.Domain.Entities;
using Farewell.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Debug.ASP.Infrastructure.Repositories;

public class FirstDomainRepository(DbContext context) 
    : EFRepository<FirstDomainEntity>(context)
{
}