using System.Linq.Expressions;
using Farewell.Abstractions.Domain;
using Farewell.Abstractions.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Infrastructure;

public abstract class EFRepository<TEntity, TUniqueKey>(DbContext context) : IQueryableRepository<TEntity, TUniqueKey>
    where TEntity : DomainEntity<TUniqueKey>
    where TUniqueKey : IComparable<TUniqueKey>
{
    public virtual DbContext Context => context;
    public virtual DbSet<TEntity> DbSet { get; } = context.Set<TEntity>();
    
    public IUnitOfWork GetUnitOfWork() => new EFUnitOfWork(context);
    
    public Task SaveChangesAsync(CancellationToken ct = default) => 
        context.SaveChangesAsync(ct);

    public IQueryable<TEntity> GetQuery() => DbSet.AsNoTracking();

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default) =>
        (await DbSet.AddAsync(entity, ct)).Entity;

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default) =>
        DbSet.AddRangeAsync(entities, ct);

    public Task<bool> AllAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) =>
        query.AllAsync(predicate, ct);

    public Task<bool> AnyAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        query.AnyAsync(ct);

    public Task<bool> AnyAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) =>
        query.AnyAsync(predicate, ct);

    public Task<float?> AverageAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, float?>> selector,
        CancellationToken ct = default) =>
        query.AverageAsync(selector, ct);
    
    public Task<double?> AverageAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, double?>> selector,
        CancellationToken ct = default) =>
        query.AverageAsync(selector, ct);
    
    public Task<decimal?> AverageAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, decimal?>> selector,
        CancellationToken ct = default) =>
        query.AverageAsync(selector, ct);

    public Task<bool> ContainsAsync<TResult>(IQueryable<TResult> query, TResult item, CancellationToken ct = default) =>
        query.ContainsAsync(item, ct);

    public Task<int> CountAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        query.CountAsync(ct);

    public Task<int> CountAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) =>
        query.CountAsync(predicate, ct);

    public Task<long> LongCountAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        query.LongCountAsync(ct);

    public Task<long> LongCountAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate,
        CancellationToken ct = default) =>
        query.LongCountAsync(predicate, ct);

    public Task<TMax> MaxAsync<TResult, TMax>(IQueryable<TResult> query, Expression<Func<TResult, TMax>> selector, CancellationToken ct = default) =>
        query.MaxAsync(selector, ct);

    public Task<TMin> MinAsync<TResult, TMin>(IQueryable<TResult> query, Expression<Func<TResult, TMin>> selector, CancellationToken ct = default) =>
        query.MinAsync(selector, ct);

    public Task<float?> SumAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, float?>> selector, CancellationToken ct = default) =>
        query.SumAsync(selector, ct);
    
    public Task<double?> SumAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, double?>> selector, CancellationToken ct = default) =>
        query.SumAsync(selector, ct);
    
    public Task<decimal?> SumAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, decimal?>> selector, CancellationToken ct = default) => 
        query.SumAsync(selector, ct);

    public Task<TResult> ElementAtAsync<TResult>(IQueryable<TResult> query, int index, CancellationToken ct = default) =>
        query.ElementAtAsync(index, ct);

    public Task<TResult?> ElementAtOrDefaultAsync<TResult>(IQueryable<TResult> query, int index, CancellationToken ct = default) =>
        query.ElementAtOrDefaultAsync(index, ct);

    public Task<TResult> FirstAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        query.FirstAsync(ct);

    public Task<TResult> FirstAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) =>
        query.FirstAsync(predicate, ct);

    public Task<TResult?> FirstOrDefaultAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        query.FirstOrDefaultAsync(ct);

    public Task<TResult?> FirstOrDefaultAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate,
        CancellationToken ct = default) =>
        query.FirstOrDefaultAsync(predicate, ct);

    public Task<TResult> LastAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        query.LastAsync(ct);

    public Task<TResult> LastAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) =>
        query.LastAsync(predicate, ct);

    public Task<TResult?> LastOrDefaultAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        query.LastOrDefaultAsync(ct);

    public Task<TResult?> LastOrDefaultAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate,
        CancellationToken ct = default) =>
        query.LastOrDefaultAsync(predicate, ct);

    public Task<TResult> SingleAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        query.SingleAsync(ct);

    public Task<TResult> SingleAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) =>
        query.SingleAsync(predicate, ct);

    public Task<TResult?> SingleOrDefaultAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        query.SingleOrDefaultAsync(ct);

    public Task<TResult?> SingleOrDefaultAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate,
        CancellationToken ct = default) =>
        query.SingleOrDefaultAsync(predicate, ct);

    public Task ForEachAsync<TResult>(IQueryable<TResult> query, Action<TResult> action, CancellationToken ct = default) =>
        query.ForEachAsync(action, ct);

    public Task<int> ExecuteDeleteAsync(IQueryable<TEntity> query,
        CancellationToken ct = default) =>
        query.ExecuteDeleteAsync(ct);

    public async Task<int> ExecuteUpdateAsync<TUpdateMap>(IQueryable<TEntity> query,
        TUpdateMap updateMap,
        CancellationToken ct = default) => 
        await query.ExecuteUpdateAsync(UpdateSetterGenerator<TEntity, TUniqueKey, TUpdateMap>.Compiled.Invoke(updateMap), ct);

    public Task<TResult[]> ToArrayAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        query.ToArrayAsync(ct);

    public Task<Dictionary<TKey, TResult>> ToDictionaryAsync<TResult, TKey>(IQueryable<TResult> query, Func<TResult, TKey> keySelector,
        CancellationToken ct = default) where TKey : notnull =>
        query.ToDictionaryAsync(keySelector, ct);

    public Task<HashSet<TResult>> ToHashSetAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        query.ToHashSetAsync(ct);

    public Task<List<TResult>> ToListAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        query.ToListAsync(ct);
}