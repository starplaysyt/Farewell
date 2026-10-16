namespace Farewell.DI.Exceptions;

public class MultiplePublicConstructorsException(Type type, int constructorsCount)
    : Exception($"Type '{type.FullName}' has {constructorsCount} public constructors. " +
                "Only one public constructor is allowed.")
{
}