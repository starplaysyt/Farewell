using System.Linq.Expressions;
using System.Reflection;
using Farewell.Mapping.Exceptions;

namespace Farewell.Mapping.Internal;

internal static class PropertyMappingBuilder
{
    public static Func<TSource, TResult> Build<TSource, TResult>()
    {
        var sourceType = typeof(TSource);
        var targetType = typeof(TResult);

        var ctor = targetType.GetConstructor(
            BindingFlags.Public | BindingFlags.Instance,
            binder: null,
            types: Type.EmptyTypes,
            modifiers: null);

        if (ctor is null)
            throw new MappingCompilationException(sourceType, targetType,
                "target type has no accessible parameterless constructor — " +
                "use [MapToConstructor] on a constructor or use a record type");

        var sourceProps = sourceType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name);

        var targetProps = targetType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToArray();

        var sourceParam = Expression.Parameter(sourceType, "source");

        var bindings = new List<MemberBinding>();

        foreach (var targetProp in targetProps)
        {
            var isRequired = IsRequired(targetProp);
            var nfInnerType = TypeConversions.GetNullableFieldInnerType(targetProp.PropertyType);
            var isNullableField = nfInnerType is not null;

            if (sourceProps.TryGetValue(targetProp.Name, out var sourceProp))
            {
                var sourceAccess = Expression.Property(sourceParam, sourceProp);
                var conversion = TypeConversions.TryConvert(
                    sourceAccess, sourceProp.PropertyType, targetProp.PropertyType);

                if (conversion is null)
                {
                    throw new MappingCompilationException(sourceType, targetType,
                        $"property '{targetProp.Name}' type mismatch — " +
                        $"source is '{sourceProp.PropertyType.Name}', " +
                        $"target is '{targetProp.PropertyType.Name}', " +
                        $"no implicit conversion exists");
                }

                bindings.Add(Expression.Bind(targetProp, conversion));
            }
            else
            {
                var missingExpr = TypeConversions.ResolveMissingMember(
                    sourceType, targetType,
                    targetProp.Name, targetProp.PropertyType,
                    isRequired,
                    allowDefault: true,
                    memberKind: "property");

                bindings.Add(Expression.Bind(targetProp, missingExpr));
            }
        }

        var newExpr = Expression.New(ctor);
        var memberInit = Expression.MemberInit(newExpr, bindings);

        var lambda = Expression.Lambda<Func<TSource, TResult>>(memberInit, sourceParam);
        return lambda.Compile();
    }

    private static bool IsRequired(PropertyInfo property)
    {
        return property
                   .GetCustomAttributes(inherit: true)
                   .Any(attr =>
                       attr.GetType().FullName ==
                       "System.Runtime.CompilerServices.RequiredMemberAttribute")
               || property.GetCustomAttributesData()
                   .Any(a => a.AttributeType.FullName ==
                             "System.Runtime.CompilerServices.RequiredMemberAttribute");
    }
}