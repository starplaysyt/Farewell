namespace Farewell.Abstractions.Exceptions;

public class DIServiceResolvingException(Type serviceType)
    : Exception($"Failed to resolve service {serviceType}");
    
public class DIInvalidImplementation(Type serviceType, params Type[] implementationTypes)
    : Exception($"Given service {serviceType} does not implement {string.Join(" or ", implementationTypes.Select(t => t.FullName))}");