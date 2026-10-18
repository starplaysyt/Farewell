using System.Collections;
using System.Linq.Expressions;
using Farewell.Abstractions.Infrastructure;
using Farewell.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Farewell.Infrastructure;

public class EFRepositoryUsagesAnalyzer<TEntity, TContext>(
    IQueryableRepository<TEntity> repository) : IRepositoryUsagesAnalyzer<TEntity>
    where TEntity : class
    where TContext : DbContext
{
    public async Task<int> CountDependentsAsync<TKey>(TKey id, CancellationToken ct = default)
        where TKey : IComparable<TKey>
    {
        var context = GetDbContext();
        var entityType = ResolvePrincipalKey(context);

        var total = 0;

        foreach (var fk in GetReferencingForeignKeys(entityType))
        {
            var query = BuildFilteredQuery(context, fk, id);
            total += await ExecuteCountAsync(query, fk.DeclaringEntityType.ClrType, ct).ConfigureAwait(false);
        }

        return total;
    }

    public async Task<Dictionary<Type, object[]>> GetDependentsAsync<TKey>(
        TKey id, CancellationToken ct = default)
        where TKey : IComparable<TKey>
    {
        var context = GetDbContext();
        var entityType = ResolvePrincipalKey(context);

        var result = new Dictionary<Type, object[]>();

        foreach (var fk in GetReferencingForeignKeys(entityType))
        {
            var dependentType = fk.DeclaringEntityType.ClrType;
            var query = BuildFilteredQuery(context, fk, id);
            var items = await ExecuteToArrayAsync(query, dependentType, ct).ConfigureAwait(false);

            if (items.Length == 0)
                continue;

            result[dependentType] = result.TryGetValue(dependentType, out var existing)
                ? [.. existing, .. items]
                : items;
        }

        return result;
    }

    private TContext GetDbContext()
    {
        if (repository is not EFRepository<TEntity, TContext> efRepo)
            throw new InvalidOperationException(
                $"Repository for {typeof(TEntity).Name} " +
                $"is not EFRepository<{typeof(TEntity).Name}, {typeof(TContext).Name}>.");

        return (TContext)efRepo.Context;
    }

    private static IEntityType ResolvePrincipalKey(TContext context)
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity))
            ?? throw new InvalidOperationException(
                $"{typeof(TEntity).Name} is not registered in the EF model.");

        var pk = entityType.FindPrimaryKey()
            ?? throw new InvalidOperationException(
                $"No primary key defined for {typeof(TEntity).Name}.");

        if (pk.Properties.Count != 1)
            throw new InvalidOperationException(
                $"{typeof(TEntity).Name} has composite primary key — " +
                $"not supported by RepositoryAnalyzer<TEntity, TKey, TContext>.");

        return entityType;
    }

    private static IEnumerable<IForeignKey> GetReferencingForeignKeys(IEntityType entityType)
        => entityType.GetReferencingForeignKeys()
            .Where(fk => !fk.IsOwnership);

    private static IQueryable BuildFilteredQuery<TKey>(
        DbContext context,
        IForeignKey fk,
        TKey id)
        where TKey : IComparable<TKey>
    {
        var dependentType = fk.DeclaringEntityType.ClrType;
        
        if (fk.Properties.Count != 1 || fk.PrincipalKey.Properties.Count != 1)
            throw new InvalidOperationException(
                $"Composite FK from {dependentType.Name} is not supported.");

        var fkProp = fk.Properties[0];
        
        var dbSet = (IQueryable)EFReflectionCache.SetMethod
            .MakeGenericMethod(dependentType)
            .Invoke(context, null)!;
        
        var param = Expression.Parameter(dependentType, "x");
        var member = BuildMemberAccess(param, fkProp);
        var constant = BuildConstant(id, fkProp.ClrType);
        var predicate = Expression.Lambda(Expression.Equal(member, constant), param);
        
        var filtered = (IQueryable)EFReflectionCache.WhereMethod
            .MakeGenericMethod(dependentType)
            .Invoke(null, [dbSet, predicate])!;
        
        filtered = (IQueryable)EFReflectionCache.AsNoTrackingMethod
            .MakeGenericMethod(dependentType)
            .Invoke(null, [filtered])!;

        return filtered;
    }

    private static Expression BuildMemberAccess(ParameterExpression param, IProperty property)
    {
        if (property.PropertyInfo is not null)
            return Expression.Property(param, property.PropertyInfo);
        
        return Expression.Call(
            EFReflectionCache.EfPropertyMethod.MakeGenericMethod(property.ClrType),
            param,
            Expression.Constant(property.Name));
    }

    private static Expression BuildConstant(object? value, Type targetType)
    {
        if (value is null)
            return Expression.Constant(null, targetType);

        var sourceType = value.GetType();

        if (sourceType == targetType)
            return Expression.Constant(value, targetType);
        
        var converted = Convert.ChangeType(
            value,
            Nullable.GetUnderlyingType(targetType) ?? targetType);

        return Expression.Constant(converted, targetType);
    }

    private static async Task<int> ExecuteCountAsync(
        IQueryable query, Type elementType, CancellationToken ct)
    {
        var method = EFReflectionCache.CountAsyncMethod.MakeGenericMethod(elementType);
        return await ((Task<int>)method.Invoke(null, [query, ct])!).ConfigureAwait(false);
    }

    private static async Task<object[]> ExecuteToArrayAsync(
        IQueryable query, Type elementType, CancellationToken ct)
    {
        var method = EFReflectionCache.ToListAsyncMethod.MakeGenericMethod(elementType);
        var task = (Task)method.Invoke(null, [query, ct])!;
        await task.ConfigureAwait(false);

        var result = (IEnumerable)task.GetType().GetProperty("Result")!.GetValue(task)!;
        return [.. result.Cast<object>()];
    }
}