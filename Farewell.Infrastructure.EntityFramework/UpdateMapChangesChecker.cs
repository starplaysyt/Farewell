using System.Linq.Expressions;
using System.Reflection;
using Farewell.Abstractions.Domain;

namespace Farewell.Infrastructure;

public static class UpdateMapHasChangesChecker<TUpdateMap>
{
    public static readonly Func<TUpdateMap, bool> Compiled = Build();

    private static Func<TUpdateMap, bool> Build()
    {
        var updateMapParam = Expression.Parameter(typeof(TUpdateMap), "updateMap");

        var mapProperties =
            typeof(TUpdateMap).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var combinedCheck = mapProperties
            .Select(mapProperty => BuildPropertyCheck(mapProperty, updateMapParam))
            .OfType<Expression>()
            .Aggregate<Expression, Expression?>(null, (current, check) => current is null
                ? check
                : Expression.OrElse(current, check));

        var body = combinedCheck ?? Expression.Constant(false);

        var lambda = Expression.Lambda<Func<TUpdateMap, bool>>(body, updateMapParam);
        return lambda.Compile();
    }

    private static BinaryExpression? BuildPropertyCheck(
        PropertyInfo mapProperty,
        ParameterExpression updateMapParam)
    {
        var mapPropType = mapProperty.PropertyType;

        if (!IsNullableNullableField(mapPropType) && !IsNullableAssignable(mapPropType))
            return null;
        
        var access = Expression.Property(updateMapParam, mapProperty);
        return Expression.NotEqual(access, Expression.Constant(null, mapPropType));
    }

    private static bool IsNullableNullableField(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying is null)
            return false;

        return underlying.IsGenericType
               && underlying.GetGenericTypeDefinition() == typeof(NullableField<>);
    }

    private static bool IsNullableAssignable(Type type) =>
        !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;
}