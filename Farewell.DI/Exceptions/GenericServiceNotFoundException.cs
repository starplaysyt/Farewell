namespace Farewell.DI.Exceptions;

public class GenericServiceNotFoundException(Type serviceType)
    : Exception($"Unable to resolve generic service {serviceType}, service not found.")
{
}