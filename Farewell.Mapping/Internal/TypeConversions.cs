using System.Linq.Expressions;
using System.Reflection;
using Farewell.Abstractions.Domain;
using Farewell.Mapping.Exceptions;

namespace Farewell.Mapping.Internal;

internal static class TypeConversions
{
    /// <summary>
    /// Tries to convert source expr to source type
    /// </summary>
    public static Expression? TryConvert(Expression sourceExpr, Type sourceType, Type targetType)
    {
        // 1. Exact
        if (sourceType == targetType)
            return sourceExpr;

        // 2. Assignable
        if (targetType.IsAssignableFrom(sourceType))
            return Expression.Convert(sourceExpr, targetType);

        // 3. Nullable wrapping: T -> T?
        var targetUnderlying = Nullable.GetUnderlyingType(targetType);
        if (targetUnderlying is not null && sourceType == targetUnderlying)
            return Expression.Convert(sourceExpr, targetType);

        // 4. NullableField wrapping: T -> NullableField<T> (by implicit operator)
        if (IsNullableFieldOf(targetType, sourceType))
            return BuildImplicitConversion(sourceExpr, sourceType, targetType);

        // 5. T -> NullableField<T>?
        if (targetUnderlying is not null && IsNullableFieldOf(targetUnderlying, sourceType))
        {
            var toNullableField = BuildImplicitConversion(sourceExpr, sourceType, targetUnderlying);
            if (toNullableField is not null)
                return Expression.Convert(toNullableField, targetType);
        }

        // 6. T? -> NullableField<T?>?
        // sourceType = Nullable<T>, targetType = Nullable<NullableField<T?>>
        if (targetUnderlying is not null)
        {
            var nfInner = GetNullableFieldInnerType(targetType);
            if (nfInner is not null)
            {
                var innerConversion = TryConvert(sourceExpr, sourceType, nfInner);
                if (innerConversion is not null)
                {
                    var nfType = typeof(NullableField<>).MakeGenericType(nfInner);
                    var ctor = nfType.GetConstructor([nfInner])!;
                    var newNf = Expression.New(ctor, innerConversion);
                    return Expression.Convert(newNf, targetType);
                }
            }
        }

        try
        {
            return Expression.Convert(sourceExpr, targetType);
        }
        catch (InvalidOperationException)
        {
            // No conversion
        }

        return null;
    }

    public static Expression DefaultExpression(Type type)
    {
        return Expression.Default(type);
    }

    public static bool IsNullable(Type type)
    {
        // Nullable<T>
        if (Nullable.GetUnderlyingType(type) is not null)
            return true;

        // Reference type
        if (!type.IsValueType)
            return true;

        return false;
    }

    /// <summary>
    /// Gets inner type from NullableField
    /// Returns null, when type is Nullable&lt;NullableField&lt;T&gt;&gt;.
    /// </summary>
    public static Type? GetNullableFieldInnerType(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying is null)
            return null;

        if (underlying.IsGenericType &&
            underlying.GetGenericTypeDefinition() == typeof(NullableField<>))
            return underlying.GetGenericArguments()[0];

        return null;
    }

    private static bool IsNullableFieldOf(Type candidateType, Type innerType)
    {
        return candidateType.IsGenericType
               && candidateType.GetGenericTypeDefinition() == typeof(NullableField<>)
               && candidateType.GetGenericArguments()[0] == innerType;
    }

    private static Expression? BuildImplicitConversion(
        Expression sourceExpr, Type sourceType, Type targetType)
    {
        var implicitMethod = targetType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit")
            .Where(m => m.ReturnType == targetType)
            .FirstOrDefault(m =>
            {
                var p = m.GetParameters();
                return p.Length == 1 && p[0].ParameterType.IsAssignableFrom(sourceType);
            });

        implicitMethod ??= sourceType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit")
            .Where(m => m.ReturnType == targetType)
            .FirstOrDefault(m =>
            {
                var p = m.GetParameters();
                return p.Length == 1 && p[0].ParameterType.IsAssignableFrom(sourceType);
            });

        return implicitMethod is not null
            ? Expression.Convert(sourceExpr, targetType, implicitMethod)
            : null;
    }

    public static bool IsRawNullableField(Type type, out Type? innerType)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NullableField<>))
        {
            innerType = type.GetGenericArguments()[0];
            return true;
        }

        innerType = null;
        return false;
    }

    public static Expression ResolveMissingMember(
        Type sourceType,
        Type targetType,
        string memberName,
        Type memberType,
        bool isRequired,
        bool allowDefault,
        string memberKind = "property")
    {
        // 1. Required - always error
        if (isRequired)
        {
            throw new MappingCompilationException(sourceType, targetType,
                $"required {memberKind} '{memberName}' " +
                $"(type '{memberType.Name}') " +
                $"on target has no matching source {memberKind}");
        }

        // 2. Raw NullableField<T> - cant set null
        if (IsRawNullableField(memberType, out var rawInner))
        {
            throw new MappingCompilationException(sourceType, targetType,
                $"{memberKind} '{memberName}' is NullableField<{rawInner!.Name}> (non-nullable struct) " +
                $"and has no matching source {memberKind} — " +
                $"use NullableField<{rawInner.Name}>? to allow missing values");
        }

        // 3. NullableField<T>? - null
        if (GetNullableFieldInnerType(memberType) is not null)
        {
            return DefaultExpression(memberType);
        }

        // 4. Nullable (reference or Nullable<T>) - null
        if (IsNullable(memberType))
        {
            return Expression.Constant(null, memberType);
        }

        // 5. Non-nullable, non-required
        if (allowDefault)
        {
            return DefaultExpression(memberType);
        }

        // 6. Constructor parameter, non-nullable, not required - exception
        throw new MappingCompilationException(sourceType, targetType,
            $"{memberKind} '{memberName}' " +
            $"(type '{memberType.Name}') is non-nullable " +
            $"and has no matching source {memberKind}");
    }
}