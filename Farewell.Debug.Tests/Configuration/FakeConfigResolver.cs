using Farewell.Abstractions.Configuration;

namespace Farewell.Debug.Tests.Configuration;

internal sealed class FakeConfigResolver(
    Func<string, object>? resolveFunc = null,
    Action<string, object>? saveFunc = null)
    : IConfigResolver
{
    public List<string> ResolvedPaths { get; } = new();
    public List<string> SavedPaths { get; } = new();

    public T Resolve<T>(string absolutePath) where T : class
    {
        ResolvedPaths.Add(absolutePath);
        
        if (resolveFunc is not null)
            return (T)resolveFunc(absolutePath);
            
        return default;
    }

    public void SaveDefaults<T>(string absolutePath, T instance) where T : class
    {
        SavedPaths.Add(absolutePath);
        saveFunc?.Invoke(absolutePath, instance!);
    }
}
