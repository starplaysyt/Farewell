namespace Farewell.DI.Exceptions;

public class NoPublicConstructorsException(Type type) 
    : Exception($"Type '{type.FullName}' has no public constructors.")
{
}