using System.Reflection;
using Farewell.DI.Exceptions;

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
        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

        return constructors.Length switch
        {
            0 => throw new NoPublicConstructorsException(type),
            > 1 => throw new MultiplePublicConstructorsException(type, constructors.Length),
            _ => constructors[0]
        };
    }
}