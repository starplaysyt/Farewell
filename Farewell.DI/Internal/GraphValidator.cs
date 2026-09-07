using Farewell.Abstractions.DI;

namespace Farewell.DI.Internal;

internal static class GraphValidator
{
    internal static void ValidateGraph(
        Dictionary<(Type, object?), List<CompiledService>> compiled)
    {
        // checks are all deps resolvable
        foreach (var ((serviceType, key), services) in compiled)
        {
            foreach (var svc in services)
            {
                for (int i = 0; i < svc.DependencyTypes.Length; i++)
                {
                    var depType = svc.DependencyTypes[i];
                    var depKey = svc.DependencyKeys[i];

                    if (!compiled.ContainsKey((depType, depKey)))
                        throw new InvalidOperationException(
                            $"Service '{serviceType}' (key: {key ?? "none"}) " +
                            $"depends on '{depType}' (key: {depKey ?? "none"}), " +
                            "which is not registered.");
                }
            }
        }

        // singleton cannot depend on scoped
        var lifetimeChecked = new HashSet<int>();

        foreach (var ((serviceType, key), services) in compiled)
        {
            foreach (var svc in services)
            {
                if (svc.Lifetime != ServiceLifetimeType.Singleton)
                    continue;

                var path = new HashSet<int>();
                ValidateNoScopedDependency(svc, compiled, lifetimeChecked, path);
            }
        }

        // cycle dependencies
        var globalVisited = new HashSet<int>();
        var inProgress = new HashSet<int>();

        foreach (var ((serviceType, key), services) in compiled)
        {
            foreach (var svc in services)
            {
                DetectCycles(svc, compiled, globalVisited, inProgress);
            }
        }
    }

    private static void ValidateNoScopedDependency(
        CompiledService current,
        Dictionary<(Type, object?), List<CompiledService>> compiled,
        HashSet<int> checkedSet,
        HashSet<int> path)
    {
        if (!path.Add(current.SlotId)) return;
        if (!checkedSet.Add(current.SlotId)) { path.Remove(current.SlotId); return; }

        for (int i = 0; i < current.DependencyTypes.Length; i++)
        {
            var depId = (current.DependencyTypes[i], current.DependencyKeys[i]);
            if (!compiled.TryGetValue(depId, out var deps))
                continue;

            foreach (var dep in deps)
            {
                if (dep.Lifetime == ServiceLifetimeType.Scoped)
                    throw new InvalidOperationException(
                        $"Singleton '{current.ServiceType}' transitively depends on " +
                        $"scoped service '{dep.ServiceType}'. Captive dependency.");

                ValidateNoScopedDependency(dep, compiled, checkedSet, path);
            }
        }

        path.Remove(current.SlotId);
    }

    private static void DetectCycles(
        CompiledService current,
        Dictionary<(Type, object?), List<CompiledService>> compiled,
        HashSet<int> visited,
        HashSet<int> inProgress)
    {
        if (inProgress.Contains(current.SlotId))
            throw new InvalidOperationException(
                $"Circular dependency detected involving '{current.ServiceType}'.");

        if (!visited.Add(current.SlotId)) return;
        inProgress.Add(current.SlotId);

        for (int i = 0; i < current.DependencyTypes.Length; i++)
        {
            var depId = (current.DependencyTypes[i], current.DependencyKeys[i]);
            if (!compiled.TryGetValue(depId, out var deps))
                continue;

            foreach (var dep in deps)
                DetectCycles(dep, compiled, visited, inProgress);
        }

        inProgress.Remove(current.SlotId);
    }
}