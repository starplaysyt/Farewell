using Farewell.Abstractions.DI;

namespace Farewell.DI.Internal;

internal sealed class CompiledService
{
    public required int SlotId { get; init; }
    public required Type ServiceType { get; init; }
    public required Type ImplementationType { get; init; }
    public required ServiceLifetimeType Lifetime { get; init; }
    public required object? Key { get; init; }
    public required Func<IServiceProvider, object> Factory { get; init; }
    public required Type[] DependencyTypes { get; init; }
    public required object?[] DependencyKeys { get; init; }
}