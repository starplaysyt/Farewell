using System.Linq.Expressions;
using System.Reflection;
using Farewell.Mapping.Exceptions;

namespace Farewell.Mapping.Internal;

internal static class ConstructorMappingBuilder
{
    public static Func<TSource, TResult> Build<TSource, TResult>()
    {
        var sourceType = typeof(TSource);
        var targetType = typeof(TResult);

        var ctor = StrategyResolver.GetTargetConstructor(targetType);
        var ctorParams = ctor.GetParameters();

        var sourceProps = sourceType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name);

        var sourceParam = Expression.Parameter(sourceType, "source");
        var arguments = new Expression[ctorParams.Length];

        for (int i = 0; i < ctorParams.Length; i++)
        {
            var param = ctorParams[i];
            var paramName = param.Name!;
            var paramType = param.ParameterType;

            var propName = ToPascalCase(paramName);

            var nfInnerType = TypeConversions.GetNullableFieldInnerType(paramType);
            var isNullableField = nfInnerType is not null;

            if (sourceProps.TryGetValue(propName, out var sourceProp))
            {
                var sourceAccess = Expression.Property(sourceParam, sourceProp);
                var conversion = TypeConversions.TryConvert(
                    sourceAccess, sourceProp.PropertyType, paramType);

                arguments[i] = conversion ?? throw new MappingCompilationException(sourceType,
                    targetType,
                    $"constructor parameter '{paramName}' type mismatch — " +
                    $"source is '{sourceProp.PropertyType.Name}', " +
                    $"target is '{paramType.Name}', " +
                    $"no implicit conversion exists");
            }
            else
            {
                arguments[i] = TypeConversions.ResolveMissingMember(
                    sourceType, targetType,
                    paramName, paramType,
                    isRequired: false,
                    allowDefault: false,
                    memberKind: "constructor parameter");
            }
        }

        var newExpr = Expression.New(ctor, arguments);
        var lambda = Expression.Lambda<Func<TSource, TResult>>(newExpr, sourceParam);
        return lambda.Compile();
    }

    private static string ToPascalCase(string name)
    {
        if (string.IsNullOrEmpty(name) || char.IsUpper(name[0]))
            return name;

        return char.ToUpperInvariant(name[0]) + name[1..];
    }
}