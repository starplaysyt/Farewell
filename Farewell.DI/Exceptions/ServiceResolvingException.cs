namespace Farewell.DI.Exceptions;

public enum ServiceResolvingExceptionType
{
    ScopedFromRoot
}

public class ServiceResolvingException(
    Type serviceType,
    object? key,
    ServiceResolvingExceptionType exceptionType)
    : Exception(exceptionType switch
    {
        ServiceResolvingExceptionType.ScopedFromRoot =>
            $"Cannot resolve keyed scoped service '{serviceType}' " +
            (key is not null ? $"(key: '{key}') " : "") + "from root provider.",
        _ => "Undefined exception."
    })
{
}