using System.Linq.Expressions;
using System.Reflection;
using Farewell.Abstractions.Domain;
using Microsoft.EntityFrameworkCore.Query;

namespace Farewell.EF;

public static class UpdateSetterGenerator<TEntity, TKey, TUpdateMap>
    where TEntity : DomainEntity<TKey>
    where TKey : IComparable<TKey>
{
    private static readonly MethodInfo SetPropertyDefinition = ResolveSetPropertyDefinition();
    private static readonly PropertyInfo NullableFieldIsSetProp = ResolveNullableFieldIsSet();
    private static readonly PropertyInfo NullableFieldValueProp = ResolveNullableFieldValue();

    public static readonly Func<TUpdateMap, Action<UpdateSettersBuilder<TEntity>>> Compiled = Build();

    private static Func<TUpdateMap, Action<UpdateSettersBuilder<TEntity>>> Build()
    {
        var updateMapParam = Expression.Parameter(typeof(TUpdateMap), "updateMap");
        var builderParam = Expression.Parameter(typeof(UpdateSettersBuilder<TEntity>), "builder");

        var statements = new List<Expression>();

        var mapProperties = typeof(TUpdateMap).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var mapProperty in mapProperties)
        {
            var stmt = BuildPropertyAssignment(mapProperty, updateMapParam, builderParam);
            if (stmt is not null)
                statements.Add(stmt);
        }

        Expression body = statements.Count > 0
            ? Expression.Block(statements)
            : Expression.Empty();

        var innerLambda = Expression.Lambda<Action<UpdateSettersBuilder<TEntity>>>(body, builderParam);
        var outerLambda = Expression.Lambda<Func<TUpdateMap, Action<UpdateSettersBuilder<TEntity>>>>(
            innerLambda,
            updateMapParam);

        return outerLambda.Compile();
    }

    private static Expression? BuildPropertyAssignment(
        PropertyInfo mapProperty,
        ParameterExpression updateMapParam,
        ParameterExpression builderParam)
    {
        var entityProp = typeof(TEntity).GetProperty(mapProperty.Name, BindingFlags.Public | BindingFlags.Instance);
        if (entityProp is null)
            return null;

        var mapPropType = mapProperty.PropertyType;
        var entityPropType = entityProp.PropertyType;

        var nullableFieldArg = GetNullableFieldInnerType(mapPropType);

        if (nullableFieldArg is not null)
        {
            return BuildNullableFieldAssignment(
                mapProperty,
                mapPropType,
                nullableFieldArg,
                entityProp,
                entityPropType,
                updateMapParam,
                builderParam);
        }
        else
        {
            return BuildStandardNullableAssignment(
                mapProperty,
                mapPropType,
                entityProp,
                entityPropType,
                updateMapParam,
                builderParam);
        }
    }

    private static Expression BuildNullableFieldAssignment(
        PropertyInfo mapProperty,
        Type mapPropType,
        Type innerType,
        PropertyInfo entityProp,
        Type entityPropType,
        ParameterExpression updateMapParam,
        ParameterExpression builderParam)
    {
        var mapPropAccess = Expression.Property(updateMapParam, mapProperty);
        var isSetAccess = Expression.Property(mapPropAccess, NullableFieldIsSetProp);
        var valueAccess = Expression.Property(mapPropAccess, NullableFieldValueProp);
        
        Expression valueExpr = valueAccess;
        if (valueAccess.Type != entityPropType)
        {
            valueExpr = Expression.Convert(valueAccess, entityPropType);
        }
        
        var propertySelector = BuildPropertySelector(entityProp);
        var expressionType = typeof(Expression<>).MakeGenericType(
            typeof(Func<,>).MakeGenericType(typeof(TEntity), entityPropType));
        var selectorConstant = Expression.Constant(propertySelector, expressionType);
        
        var setMethod = SetPropertyDefinition.MakeGenericMethod(entityPropType);
        var setCall = Expression.Call(builderParam, setMethod, selectorConstant, valueExpr);
        
        return Expression.IfThen(isSetAccess, setCall);
    }
    
    private static Expression BuildStandardNullableAssignment(
        PropertyInfo mapProperty,
        Type mapPropType,
        PropertyInfo entityProp,
        Type entityPropType,
        ParameterExpression updateMapParam,
        ParameterExpression builderParam)
    {
        if (!IsNullableAssignable(mapPropType))
            throw new InvalidOperationException(
                $"Property '{mapProperty.Name}' of type '{mapPropType.Name}' is not nullable. " +
                "Use Nullable<T>, a reference type, or NullableField<T>.");

        var mapPropAccess = Expression.Property(updateMapParam, mapProperty);
        var nullCheck = Expression.NotEqual(mapPropAccess, Expression.Constant(null, mapPropType));

        Expression valueExpr = mapPropAccess;
        if (mapPropType != entityPropType)
        {
            valueExpr = Expression.Convert(mapPropAccess, entityPropType);
        }

        var propertySelector = BuildPropertySelector(entityProp);
        var expressionType = typeof(Expression<>).MakeGenericType(
            typeof(Func<,>).MakeGenericType(typeof(TEntity), entityPropType));
        var selectorConstant = Expression.Constant(propertySelector, expressionType);

        var setMethod = SetPropertyDefinition.MakeGenericMethod(entityPropType);
        var setCall = Expression.Call(builderParam, setMethod, selectorConstant, valueExpr);

        return Expression.IfThen(nullCheck, setCall);
    }
    
    private static LambdaExpression BuildPropertySelector(PropertyInfo entityProp)
    {
        var entityParam = Expression.Parameter(typeof(TEntity), "x");
        var delegateType = typeof(Func<,>).MakeGenericType(typeof(TEntity), entityProp.PropertyType);
        return Expression.Lambda(
            delegateType,
            Expression.Property(entityParam, entityProp),
            entityParam);
    }

    private static Type? GetNullableFieldInnerType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NullableField<>))
            return type.GetGenericArguments()[0];
        return null;
    }

    private static MethodInfo ResolveSetPropertyDefinition()
    {
        var builderType = typeof(UpdateSettersBuilder<TEntity>);
        var method = builderType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m =>
            {
                if (m.Name != "SetProperty" || !m.IsGenericMethodDefinition) return false;
                var p = m.GetParameters();
                return p.Length == 2 && p[1].ParameterType == m.GetGenericArguments()[0];
            });
        return method ?? throw new InvalidOperationException(
            $"SetProperty<T>(Expression<Func<TEntity,T>>, T) not found on '{builderType.Name}'.");
    }

    private static PropertyInfo ResolveNullableFieldIsSet()
    {
        // Берём из любого закрытого NullableField<> — PropertyInfo одинаковый для всех
        var sampleType = typeof(NullableField<>).MakeGenericType(typeof(int));
        return sampleType.GetProperty("IsSet")
               ?? throw new InvalidOperationException("NullableField<T>.IsSet not found.");
    }

    private static PropertyInfo ResolveNullableFieldValue()
    {
        var sampleType = typeof(NullableField<>).MakeGenericType(typeof(int));
        return sampleType.GetProperty("Value")
               ?? throw new InvalidOperationException("NullableField<T>.Value not found.");
    }

    private static bool IsNullableAssignable(Type type) =>
        !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;
}