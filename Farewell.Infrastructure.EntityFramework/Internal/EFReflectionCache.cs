using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Farewell.Infrastructure.Internal;

internal static class EFReflectionCache
{
    public static readonly MethodInfo SetMethod =
        typeof(DbContext).GetMethods()
            .First(m => m.Name == "Set"
                        && m.IsGenericMethod
                        && m.GetParameters().Length == 0);

    public static readonly MethodInfo WhereMethod =
        typeof(Queryable).GetMethods()
            .First(m => m.Name == "Where"
                        && m.GetParameters().Length == 2
                        && m.GetParameters()[1].ParameterType
                            .GetGenericArguments()[0]
                            .GetGenericArguments().Length == 2);

    public static readonly MethodInfo AsNoTrackingMethod =
        typeof(EntityFrameworkQueryableExtensions).GetMethods()
            .First(m => m.Name == "AsNoTracking" && m.IsGenericMethod);

    public static readonly MethodInfo CountAsyncMethod =
        typeof(EntityFrameworkQueryableExtensions).GetMethods()
            .First(m => m.Name == "CountAsync"
                        && m.GetParameters().Length == 2
                        && m.GetParameters()[1].ParameterType == typeof(CancellationToken));

    public static readonly MethodInfo ToListAsyncMethod =
        typeof(EntityFrameworkQueryableExtensions).GetMethods()
            .First(m => m.Name == "ToListAsync"
                        && m.GetParameters().Length == 2);

    public static readonly MethodInfo EfPropertyMethod =
        typeof(EF).GetMethod(nameof(EF.Property))!;
}