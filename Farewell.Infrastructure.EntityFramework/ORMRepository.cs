using System.Linq.Expressions;
using Farewell.Abstractions.Domain;
using Farewell.Abstractions.Infrastructure;
using Farewell.EF;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Infrastructure;

public abstract class ORMRepository<TEntity, TUniqueKey>(DbContext context) : IORMRepository<TEntity, TUniqueKey>
    where TEntity : DomainEntity<TUniqueKey>
    where TUniqueKey : IComparable<TUniqueKey>
{
    public virtual DbSet<TEntity> DbSet { get; } = context.Set<TEntity>();

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);

    public async Task<ITransactionInfo> BeginTransactionAsync(CancellationToken ct = default) =>
        new TransactionInfo {  DbTransaction = await context.Database.BeginTransactionAsync(ct) };

    public async Task CommitTransactionAsync(ITransactionInfo info, CancellationToken ct = default)
    {
        if (info is not TransactionInfo transactionInfo)
            throw new InvalidOperationException("Invalid transaction info");
        if (transactionInfo.DbTransaction is null)
            throw new InvalidOperationException("This transaction has been already committed/rollbacked");
        
        await transactionInfo.DbTransaction.CommitAsync(ct);
        await transactionInfo.DisposeAsync();
        transactionInfo.DbTransaction = null!;
    }

    public async Task RollbackTransactionAsync(ITransactionInfo info, CancellationToken ct = default)
    {
        if (info is not TransactionInfo transactionInfo)
            throw new InvalidOperationException("Invalid transaction info");
        if (transactionInfo.DbTransaction is null)
            throw new InvalidOperationException("This transaction has been already committed/rollbacked");
        
        await transactionInfo.DbTransaction.RollbackAsync(ct);
        await transactionInfo.DisposeAsync();
        transactionInfo.DbTransaction = null!;
    }
    
    public IQueryable<TEntity> GetQuery() => DbSet.AsNoTracking();

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default) =>
        (await DbSet.AddAsync(entity, ct)).Entity;

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default) =>
        await DbSet.AddRangeAsync(entities, ct);

    public async Task<bool> AllAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) =>
        await query.AllAsync(predicate, ct);

    public async Task<bool> AnyAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        await query.AnyAsync(ct);

    public async Task<bool> AnyAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) =>
        await query.AnyAsync(predicate, ct);

    public async Task<float?> AverageAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, float?>> selector,
        CancellationToken ct = default) =>
        await query.AverageAsync(selector, ct);
    
    public async Task<double?> AverageAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, double?>> selector,
        CancellationToken ct = default) =>
        await query.AverageAsync(selector, ct);
    
    public async Task<decimal?> AverageAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, decimal?>> selector,
        CancellationToken ct = default) =>
        await query.AverageAsync(selector, ct);

    public async Task<bool> ContainsAsync<TResult>(IQueryable<TResult> query, TResult item, CancellationToken ct = default) =>
        await query.ContainsAsync(item, ct);

    public async Task<int> CountAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        await query.CountAsync(ct);

    public async Task<int> CountAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) =>
        await query.CountAsync(predicate, ct);

    public async Task<long> LongCountAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        await query.LongCountAsync(ct);

    public async Task<long> LongCountAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate,
        CancellationToken ct = default) =>
        await query.LongCountAsync(predicate, ct);

    public async Task<TMax> MaxAsync<TResult, TMax>(IQueryable<TResult> query, Expression<Func<TResult, TMax>> selector, CancellationToken ct = default) =>
        await query.MaxAsync(selector, ct);

    public async Task<TMin> MinAsync<TResult, TMin>(IQueryable<TResult> query, Expression<Func<TResult, TMin>> selector, CancellationToken ct = default) =>
        await query.MinAsync(selector, ct);

    public async Task<float?> SumAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, float?>> selector, CancellationToken ct = default) =>
        await query.SumAsync(selector, ct);
    
    public async Task<double?> SumAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, double?>> selector, CancellationToken ct = default) =>
        await query.SumAsync(selector, ct);
    
    public async Task<decimal?> SumAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, decimal?>> selector, CancellationToken ct = default) =>
        await query.SumAsync(selector, ct);

    public async Task<TResult> ElementAtAsync<TResult>(IQueryable<TResult> query, int index, CancellationToken ct = default) =>
        await query.ElementAtAsync(index, ct);

    public async Task<TResult?> ElementAtOrDefaultAsync<TResult>(IQueryable<TResult> query, int index, CancellationToken ct = default) =>
        await query.ElementAtOrDefaultAsync(index, ct);

    public async Task<TResult> FirstAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        await query.FirstAsync(ct);

    public async Task<TResult> FirstAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) =>
        await query.FirstAsync(predicate, ct);

    public async Task<TResult?> FirstOrDefaultAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        await query.FirstOrDefaultAsync(ct);

    public async Task<TResult?> FirstOrDefaultAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate,
        CancellationToken ct = default) =>
        await query.FirstOrDefaultAsync(predicate, ct);

    public async Task<TResult> LastAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        await query.LastAsync(ct);

    public async Task<TResult> LastAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) =>
        await query.LastAsync(predicate, ct);

    public async Task<TResult?> LastOrDefaultAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        await query.LastOrDefaultAsync(ct);

    public async Task<TResult?> LastOrDefaultAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate,
        CancellationToken ct = default) =>
        await query.LastOrDefaultAsync(predicate, ct);

    public async Task<TResult> SingleAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        await query.SingleAsync(ct);

    public async Task<TResult> SingleAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate, CancellationToken ct = default) =>
        await query.SingleAsync(predicate, ct);

    public async Task<TResult?> SingleOrDefaultAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        await query.SingleOrDefaultAsync(ct);

    public async Task<TResult?> SingleOrDefaultAsync<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> predicate,
        CancellationToken ct = default) =>
        await query.SingleOrDefaultAsync(predicate, ct);

    public async Task ForEachAsync<TResult>(IQueryable<TResult> query, Action<TResult> action, CancellationToken ct = default) =>
        await query.ForEachAsync(action, ct);

    public async Task<int> ExecuteDeleteAsync(IQueryable<TEntity> query,
        CancellationToken ct = default) =>
        await query.ExecuteDeleteAsync(ct);

    public async Task<int> ExecuteUpdateAsync<TUpdateMap>(IQueryable<TEntity> query,
        TUpdateMap updateMap,
        CancellationToken ct = default) =>
        await query.ExecuteUpdateAsync(UpdateSetterGenerator<TEntity, TUniqueKey, TUpdateMap>.Compiled.Invoke(updateMap), ct);

    public async Task<TResult[]> ToArrayAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        await query.ToArrayAsync(ct);

    public async Task<Dictionary<TKey, TResult>> ToDictionaryAsync<TResult, TKey>(IQueryable<TResult> query, Func<TResult, TKey> keySelector,
        CancellationToken ct = default) where TKey : notnull =>
        await query.ToDictionaryAsync(keySelector, ct);

    public async Task<HashSet<TResult>> ToHashSetAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        await query.ToHashSetAsync(ct);

    public async Task<List<TResult>> ToListAsync<TResult>(IQueryable<TResult> query, CancellationToken ct = default) =>
        await query.ToListAsync(ct);
}