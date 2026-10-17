using System.Linq.Expressions;
using Farewell.Abstractions.Domain;
using Farewell.Abstractions.Infrastructure;

namespace Farewell.Debug.Tests.ValidationInfrastructureTests;

public sealed class InMemoryQueryableRepository<TEntity> : IQueryableRepository<TEntity>
    where TEntity : DomainEntity
{
    private readonly List<TEntity> _store = [];

    public void Seed(params TEntity[] entities) => _store.AddRange(entities);

    public IQueryable<TEntity> GetQuery() => _store.AsQueryable();

    public Task<bool> AnyAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default)
        => Task.FromResult(query.Any());

    public Task<bool> AnyAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult(query.Any(predicate));

    public Task<int> CountAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default)
        => Task.FromResult(query.Count());

    public Task<int> CountAsync<TResult>(IQueryable<TResult> query,
        Expression<Func<TResult, bool>> predicate, CancellationToken ct = default)
        => Task.FromResult(query.Count(predicate));

    public IUnitOfWork GetUnitOfWork() => throw new NotImplementedException();
    public Task SaveChangesAsync(CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default) => throw new NotImplementedException();
    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<bool> AllAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<float?> AverageAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, float?>> selector, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<double?> AverageAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, double?>> selector, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<decimal?> AverageAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, decimal?>> selector, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<bool> ContainsAsync<TResult>(IQueryable<TResult> query, TResult item, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<long> LongCountAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<long> LongCountAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TMax> MaxAsync<TResult, TMax>(IQueryable<TResult> query, Expression<Func<TResult, TMax>> selector, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TMin> MinAsync<TResult, TMin>(IQueryable<TResult> query, Expression<Func<TResult, TMin>> selector, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<float?> SumAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, float?>> selector, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<double?> SumAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, double?>> selector, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<decimal?> SumAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, decimal?>> selector, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult> ElementAtAsync<TResult>(IQueryable<TResult> query, int index, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult?> ElementAtOrDefaultAsync<TResult>(IQueryable<TResult> query, int index, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult> FirstAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult> FirstAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult?> FirstOrDefaultAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult?> FirstOrDefaultAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult> LastAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult> LastAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult?> LastOrDefaultAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult?> LastOrDefaultAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult> SingleAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult> SingleAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult?> SingleOrDefaultAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult?> SingleOrDefaultAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) => throw new NotImplementedException();
    public Task ForEachAsync<TResult>(IQueryable<TResult> query, Action<TResult> action, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<int> ExecuteDeleteAsync(IQueryable<TEntity> query, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<int> ExecuteUpdateAsync<TUpdateMap>(IQueryable<TEntity> query, TUpdateMap updateMap, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<TResult[]> ToArrayAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<Dictionary<TKey, TResult>> ToDictionaryAsync<TResult, TKey>(IQueryable<TResult> query, Func<TResult, TKey> keySelector, CancellationToken ct = default) where TKey : notnull => throw new NotImplementedException();
    public Task<HashSet<TResult>> ToHashSetAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<List<TResult>> ToListAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) => throw new NotImplementedException();
}