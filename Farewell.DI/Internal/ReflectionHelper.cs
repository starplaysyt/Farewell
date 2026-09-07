using System.Reflection;

namespace Farewell.DI.Internal;

internal class ReflectionHelper
{
    internal static Type UnwrapEnumerable(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return type.GetGenericArguments()[0];
        return type;
    }

    internal static ConstructorInfo GetSinglePublicConstructor(Type type)
    {
        var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

        if (ctors.Length == 0)
            throw new InvalidOperationException(
                $"Type '{type.FullName}' has no public constructors.");
        if (ctors.Length > 1)
            throw new InvalidOperationException(
                $"Type '{type.FullName}' has {ctors.Length} public constructors. " +
                "Only one public constructor is allowed.");

        return ctors[0];
    }
}