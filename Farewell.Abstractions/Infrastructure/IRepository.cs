using Farewell.Abstractions.Domain;

namespace Farewell.Abstractions.Infrastructure;

public interface IDomainRepository<TEntity, TKey>
    where TEntity : DomainEntity<TKey> 
    where TKey : IComparable<TKey>
{
    public Type EntityType => typeof(TEntity);
}