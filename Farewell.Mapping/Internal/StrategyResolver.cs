using System.Reflection;
using System.Runtime.CompilerServices;
using Farewell.Abstractions.Attributes.Mapping;
using Farewell.Mapping.Exceptions;

namespace Farewell.Mapping.Internal;

internal static class StrategyResolver
{
    public static MappingStrategy Resolve(Type targetType)
    {
        var markedCtor = targetType
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(c => c.IsDefined(typeof(MapToConstructorAttribute)));

        if (markedCtor is not null || IsRecord(targetType))
            return MappingStrategy.Constructor;

        return MappingStrategy.Properties;
    }

    public static ConstructorInfo GetTargetConstructor(Type targetType)
    {
        // looking for attribute marked
        var markedCtor = targetType
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(c => c.IsDefined(typeof(MapToConstructorAttribute)));

        if (markedCtor is not null)
            return markedCtor;

        // for record - primary ctor
        var ctors = targetType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

        // looking for record copy constructor
        var candidates = ctors
            .Where(c => !IsCopyConstructor(c, targetType))
            .OrderByDescending(c => c.GetParameters().Length)
            .ToArray();

        return candidates.Length > 0
            ? candidates[0]
            : throw new MappingCompilationException(
                typeof(void), targetType,
                "no accessible public constructor found");
    }

    private static bool IsRecord(Type type)
    {
        if (type.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance) is not null)
            return true;

        if (!type.IsValueType) return false;

        var printMembers = type.GetMethod("PrintMembers",
            BindingFlags.NonPublic | BindingFlags.Instance);
        return printMembers?.IsDefined(typeof(CompilerGeneratedAttribute)) == true;
    }

    private static bool IsCopyConstructor(ConstructorInfo ctor, Type type)
    {
        var parameters = ctor.GetParameters();
        return parameters.Length == 1 && parameters[0].ParameterType == type;
    }
}