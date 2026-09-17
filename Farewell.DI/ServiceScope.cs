using Farewell.Abstractions.DI;
using Farewell.DI.Internal;

namespace Farewell.DI;

public sealed class ServiceScope : IKeyedServiceProvider
{
    private readonly ServiceProvider _provider;
    private readonly Dictionary<Type, object> _scopedCache = new();
    private readonly Dictionary<(Type, object), object> _keyedScopedCache = new();
    private readonly List<IDisposable> _disposables = new();
    private readonly Dictionary<int, object> _scopedBySlot = new();
    private bool _disposed;

    internal ServiceScope(ServiceProvider provider)
    {
        _provider = provider;
    }

    public object? GetService(Type serviceType)
    {
        ThrowIfDisposed();

        // Singleton?
        var singleton = TryGetSingleton(serviceType);
        if (singleton is not null)
            return singleton;

        // Scoped?
        if (_provider.HasScopedFactory(serviceType))
        {
            if (_scopedCache.TryGetValue(serviceType, out var cached))
                return cached;

            var factory = _provider.GetScopedFactory(serviceType);
            var instance = factory(this);

            _scopedCache[serviceType] = instance;
            if (instance is IDisposable disposable)
                _disposables.Add(disposable);

            return instance;
        }

        // Transient?
        if (_provider.HasTransientFactory(serviceType))
        {
            var factory = _provider.GetTransientFactory(serviceType);
            var instance = factory(this);
            if (instance is IDisposable disposable)
                _disposables.Add(disposable);
            return instance;
        }

        // IEnumerable?
        if (serviceType.IsGenericType &&
            serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            if (_provider.HasEnumerableFactory(serviceType))
            {
                var factory = _provider.GetEnumerableFactory(serviceType);
                return factory(this);
            }

            var elementType = serviceType.GetGenericArguments()[0];
            return Array.CreateInstance(elementType, 0);
        }

        // Open generics? (fuck you)
        if (serviceType is { IsGenericType: true, IsGenericTypeDefinition: false })
        {
            var closedSvc = _provider.ResolveOpenGenericCompiled(serviceType);
            return ResolveFromCompiledService(closedSvc);
        }

        // Сосал?
        return null;
    }

    public object? GetKeyedService(Type serviceType, object key)
    {
        ThrowIfDisposed();

        var singleton = _provider.TryGetKeyedSingleton(serviceType, key);
        if (singleton is not null)
            return singleton;

        if (_provider.HasKeyedScopedFactory(serviceType, key))
        {
            var cacheKey = (serviceType, key);
            if (_keyedScopedCache.TryGetValue(cacheKey, out var cached))
                return cached;

            var factory = _provider.GetKeyedScopedFactory(serviceType, key);
            var instance = factory(this);

            _keyedScopedCache[cacheKey] = instance;
            if (instance is IDisposable disposable)
                _disposables.Add(disposable);

            return instance;
        }

        if (_provider.HasKeyedTransientFactory(serviceType, key))
        {
            var factory = _provider.GetKeyedTransientFactory(serviceType, key);
            var instance = factory(this);
            if (instance is IDisposable disposable)
                _disposables.Add(disposable);
            return instance;
        }

        return null;
    }

    internal object ResolveFromCompiledService(CompiledService svc)
    {
        switch (svc.Lifetime)
        {
            case ServiceLifetimeType.Singleton:
                return _provider.ResolveFromCompiledService(svc);

            case ServiceLifetimeType.Scoped:
                if (_scopedBySlot.TryGetValue(svc.SlotId, out var cached))
                    return cached;
                var scoped = svc.Factory(this);
                _scopedBySlot[svc.SlotId] = scoped;
                if (scoped is IDisposable sd) _disposables.Add(sd);
                return scoped;

            case ServiceLifetimeType.Transient:
                var transient = svc.Factory(this);
                if (transient is IDisposable td) _disposables.Add(td);
                return transient;

            default:
                throw new InvalidOperationException();
        }
    }

    private object? TryGetSingleton(Type serviceType) => _provider.TryGetSingleton(serviceType);

    private void ThrowIfDisposed()
    {
        if (!_disposed) return;
        throw new ObjectDisposedException(nameof(ServiceScope));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        for (var i = _disposables.Count - 1; i >= 0; i--)
        {
            try
            {
                _disposables[i].Dispose();
            }
            catch
            {
                /* хрюкни и сглотни */
            }
        }

        _disposables.Clear();
        _scopedCache.Clear();
        _keyedScopedCache.Clear();
    }
}