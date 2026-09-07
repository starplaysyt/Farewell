using Farewell.Abstractions.DI;

namespace Farewell.DI.Internal;

internal static class SingletonTopologicalSorter
{
    internal static List<CompiledService> Sort(
        Dictionary<(Type, object?), List<CompiledService>> compiled)
    {
        var singletons = compiled.Values
            .SelectMany(list => list)
            .Where(s => s.Lifetime == ServiceLifetimeType.Singleton)
            .ToList();

        var sorted = new List<CompiledService>();
        var visited = new HashSet<int>();   // ← по SlotId, не по (Type, Key)

        foreach (var svc in singletons)
            TopoVisit(svc, compiled, visited, sorted);

        return sorted;
    }

    private static void TopoVisit(
        CompiledService current,
        Dictionary<(Type, object?), List<CompiledService>> compiled,
        HashSet<int> visited,
        List<CompiledService> sorted)
    {
        if (!visited.Add(current.SlotId)) return;

        for (int i = 0; i < current.DependencyTypes.Length; i++)
        {
            var depId = (current.DependencyTypes[i], current.DependencyKeys[i]);
            if (!compiled.TryGetValue(depId, out var deps))
                continue;

            foreach (var dep in deps)
            {
                if (dep.Lifetime == ServiceLifetimeType.Singleton)
                    TopoVisit(dep, compiled, visited, sorted);
            }
        }

        sorted.Add(current);
    }
}