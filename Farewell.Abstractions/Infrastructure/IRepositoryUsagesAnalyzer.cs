using Farewell.Abstractions.Domain;

namespace Farewell.Abstractions.Infrastructure;

public interface IRepositoryUsagesAnalyzer<TEntity>
{
    Task<int> CountDependentsAsync<TKey>(TKey id, CancellationToken ct = default)
        where TKey : IComparable<TKey>;
    Task<Dictionary<Type, object[]>> GetDependentsAsync<TKey>(TKey id, CancellationToken ct = default)
        where TKey : IComparable<TKey>;
}