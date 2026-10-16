namespace Farewell.DI.Exceptions;

public enum DIValidationExceptionType
{
    None,
    NotRegistered,
    CircularDependency,
    SingletonScopedCaptiveDependency
}

public class DependencyValidationException(
    Type serviceType,
    object? serviceKey,
    Type dependencyType,
    object? dependencyKey,
    DIValidationExceptionType validationExceptionType)
    : Exception(validationExceptionType switch
        {
            DIValidationExceptionType.NotRegistered =>
                $"Service '{serviceType}' (key: {serviceKey ?? "none"}) " +
                $"depends on '{dependencyType}' (key: {dependencyKey ?? "none"}), " +
                "which is not registered.",
            DIValidationExceptionType.CircularDependency =>
                $"Circular dependency detected involving '{serviceType}'.",
            DIValidationExceptionType.SingletonScopedCaptiveDependency =>
                $"Singleton '{serviceType}' (key: {serviceKey ?? "none"}) " +
                $"transitively depends on scoped service '{dependencyType}' (key: {dependencyKey ?? "none"}). " +
                $"Captive dependency.",
            _ => "Undefined exception"
        }
    )
{
}