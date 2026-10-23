using Farewell.Abstractions.Domain;
using Farewell.Abstractions.General;
using Farewell.Abstractions.Infrastructure;
using Farewell.Abstractions.Validation;

namespace Farewell.Abstractions.Extensions;

public static class DbValidationConstraintBuilderExtensions
{
    // TODO: Add specialized ResultCode group for persistence exceptions. (unite with file exceptions?)

    /// <summary>
    /// Value must exist in DB. Builds query from value, checks AnyAsync.
    /// Code: ResultCode.NotFound.
    /// </summary>
    public static IPropertyConstraintBuilder<TValue> DbExists<TValue, TEntity>(
        this IPropertyConstraintBuilder<TValue> b,
        Func<TValue, IServiceProvider, IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder)
        where TEntity : DomainEntity
        => b.AddCheck(ResultCode.NotFound, async (value, sp, ct) =>
        {
            var repo = sp.GetRequiredService<IQueryableRepository<TEntity>>();
            var query = queryBuilder(value, sp, repo.GetQuery());
            return await repo.AnyAsync(query, ct).ConfigureAwait(false);
        });

    /// <summary>
    /// Value must NOT exist in DB. Builds query from value, checks !AnyAsync.
    /// Code: ResultCode.Conflict.
    /// </summary>
    public static IPropertyConstraintBuilder<TValue> DbNotExists<TValue, TEntity>(
        this IPropertyConstraintBuilder<TValue> b,
        Func<TValue, IServiceProvider, IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder)
        where TEntity : DomainEntity
        => b.AddCheck(ResultCode.Conflict, async (value, sp, ct) =>
        {
            var repo = sp.GetRequiredService<IQueryableRepository<TEntity>>();
            var query = queryBuilder(value, sp, repo.GetQuery());
            return !await repo.AnyAsync(query, ct).ConfigureAwait(false);
        });

    /// <summary>
    /// Value must be unique in DB. Builds query from value, checks CountAsync &lt;= 1.
    /// Code: ResultCode.Conflict.
    /// </summary>
    public static IPropertyConstraintBuilder<TValue> DbUnique<TValue, TEntity>(
        this IPropertyConstraintBuilder<TValue> b,
        Func<TValue, IServiceProvider, IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder)
        where TEntity : DomainEntity
        => b.AddCheck(ResultCode.Conflict, async (value, sp, ct) =>
        {
            var repo = sp.GetRequiredService<IQueryableRepository<TEntity>>();
            var query = queryBuilder(value, sp, repo.GetQuery());
            return await repo.CountAsync(query, ct).ConfigureAwait(false) <= 1;
        });

    /// <summary>
    /// Count of matching records must be within [min, max].
    /// Code: ResultCode.ValidationOutOfRange.
    /// </summary>
    public static IPropertyConstraintBuilder<TValue> DbCount<TValue, TEntity>(
        this IPropertyConstraintBuilder<TValue> b,
        Func<TValue, IServiceProvider, IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder,
        int min,
        int max)
        where TEntity : DomainEntity
        => b.AddCheck(ResultCode.ValidationOutOfRange, async (value, sp, ct) =>
        {
            var repo = sp.GetRequiredService<IQueryableRepository<TEntity>>();
            var query = queryBuilder(value, sp, repo.GetQuery());
            var count = await repo.CountAsync(query, ct).ConfigureAwait(false);
            return count >= min && count <= max;
        });

    /// <summary>
    /// At least one record must match the query (not tied to property value).
    /// Code: ResultCode.NotFound.
    /// </summary>
    public static IPropertyConstraintBuilder<TValue> DbAny<TValue, TEntity>(
        this IPropertyConstraintBuilder<TValue> b,
        Func<IServiceProvider, IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder)
        where TEntity : DomainEntity
        => b.AddCheck(ResultCode.NotFound, async (_, sp, ct) =>
        {
            var repo = sp.GetRequiredService<IQueryableRepository<TEntity>>();
            var query = queryBuilder(sp, repo.GetQuery());
            return await repo.AnyAsync(query, ct).ConfigureAwait(false);
        });

    /// <summary>
    /// No records must match the query (not tied to property value).
    /// Code: ResultCode.Conflict.
    /// </summary>
    public static IPropertyConstraintBuilder<TValue> DbNone<TValue, TEntity>(
        this IPropertyConstraintBuilder<TValue> b,
        Func<IServiceProvider, IQueryable<TEntity>, IQueryable<TEntity>> queryBuilder)
        where TEntity : DomainEntity
        => b.AddCheck(ResultCode.Conflict, async (_, sp, ct) =>
        {
            var repo = sp.GetRequiredService<IQueryableRepository<TEntity>>();
            var query = queryBuilder(sp, repo.GetQuery());
            return !await repo.AnyAsync(query, ct).ConfigureAwait(false);
        });
}