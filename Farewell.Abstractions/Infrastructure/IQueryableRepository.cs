using System.Linq.Expressions;
using Farewell.Abstractions.Domain;

namespace Farewell.Abstractions.Infrastructure;

public interface IQueryableRepository<TEntity, TUniqueKey>
    where TEntity : DomainEntity<TUniqueKey>
    where TUniqueKey : IComparable<TUniqueKey>
{
    IQueryable<TEntity> GetQuery();
    
    // FromSqlRawAsync - unsupported, architecture limitations
    // Bulk update with different update maps - unsupported by EntityFramework -
    // maybe extension in future
    // AsSplitQuery - useless without Include - no Include, no need in AsSplitQuery

    public IUnitOfWork GetUnitOfWork();

    Task<TEntity> AddAsync(TEntity entity,
        CancellationToken ct = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities,
        CancellationToken ct = default);

    Task<bool> AllAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, bool>> predicate,
        CancellationToken ct = default);

    Task<bool> AnyAsync<TResult>(IQueryable<TResult> query,
        CancellationToken ct = default);

    Task<bool> AnyAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, bool>> predicate,
        CancellationToken ct = default);

    Task<float?> AverageAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, float?>> selector,
        CancellationToken ct = default);

    Task<double?> AverageAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, double?>> selector,
        CancellationToken ct = default);

    Task<decimal?> AverageAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, decimal?>> selector,
        CancellationToken ct = default);

    Task<bool> ContainsAsync<TResult>(IQueryable<TResult> query, TResult item,
        CancellationToken ct = default);

    Task<int> CountAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default);

    Task<int> CountAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, bool>> predicate, CancellationToken ct = default);

    Task<long> LongCountAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default);

    Task<long> LongCountAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, bool>> predicate, CancellationToken ct = default);

    Task<TMax> MaxAsync<TResult, TMax>(IQueryable<TResult> query,
        Expression<Func<TResult, TMax>> selector, CancellationToken ct = default);

    Task<TMin> MinAsync<TResult, TMin>(IQueryable<TResult> query,
        Expression<Func<TResult, TMin>> selector, CancellationToken ct = default);
    
    Task<float?> SumAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, float?>> selector, CancellationToken ct = default);

    Task<double?> SumAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, double?>> selector, CancellationToken ct = default);

    Task<decimal?> SumAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, decimal?>> selector, CancellationToken ct = default);

    Task<TResult> ElementAtAsync<TResult>(IQueryable<TResult> query, int index,
        CancellationToken ct = default);

    Task<TResult?> ElementAtOrDefaultAsync<TResult>(IQueryable<TResult> query, int index,
        CancellationToken ct = default);

    Task<TResult> FirstAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default);

    Task<TResult> FirstAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, bool>> predicate, CancellationToken ct = default);

    Task<TResult?> FirstOrDefaultAsync<TResult>(IQueryable<TResult> query,
        CancellationToken ct = default);

    Task<TResult?> FirstOrDefaultAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, bool>> predicate, CancellationToken ct = default);

    Task<TResult> LastAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default);

    Task<TResult> LastAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, bool>> predicate, CancellationToken ct = default);

    Task<TResult?> LastOrDefaultAsync<TResult>(IQueryable<TResult> query,
        CancellationToken ct = default);

    Task<TResult?> LastOrDefaultAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, bool>> predicate, CancellationToken ct = default);

    Task<TResult> SingleAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default);

    Task<TResult> SingleAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, bool>> predicate, CancellationToken ct = default);

    Task<TResult?> SingleOrDefaultAsync<TResult>(IQueryable<TResult> query,
        CancellationToken ct = default);

    Task<TResult?> SingleOrDefaultAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, bool>> predicate, CancellationToken ct = default);

    Task ForEachAsync<TResult>(IQueryable<TResult> query, Action<TResult> action,
        CancellationToken ct = default);

    Task<int> ExecuteDeleteAsync(IQueryable<TEntity> query, CancellationToken ct = default);

    Task<int> ExecuteUpdateAsync<TUpdateMap>(IQueryable<TEntity> query, TUpdateMap updateMap,
        CancellationToken ct = default);

    Task<TResult[]> ToArrayAsync<TResult>(IQueryable<TResult> query,
        CancellationToken ct = default);

    Task<Dictionary<TKey, TResult>> ToDictionaryAsync<TResult, TKey>(IQueryable<TResult> query,
        Func<TResult, TKey> keySelector, CancellationToken ct = default)
        where TKey : notnull;

    Task<HashSet<TResult>> ToHashSetAsync<TResult>(IQueryable<TResult> query,
        CancellationToken ct = default);

    Task<List<TResult>> ToListAsync<TResult>(IQueryable<TResult> query,
        CancellationToken ct = default);
}