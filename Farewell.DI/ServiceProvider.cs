using System.Collections.Concurrent;
using System.Reflection;
using Farewell.Abstractions.DI;
using Farewell.DI.Internal;

namespace Farewell.DI;

public sealed class ServiceProvider : IScopeProvider
{
    // Singleton cashes
    private readonly Dictionary<Type, object> _singletonCache;
    private readonly Dictionary<(Type, object), object> _keyedSingletonCache;
    private readonly ConcurrentDictionary<int, object> _singletonBySlot = new();

    // Transient delegates
    private readonly Dictionary<Type, Func<IServiceProvider, object>> _transientFactories;
    private readonly Dictionary<(Type, object), Func<IServiceProvider, object>> _keyedTransientFactories;

    // Scoped delegates (only stored here)
    private readonly Dictionary<Type, Func<IServiceProvider, object>> _scopedFactories;
    private readonly Dictionary<(Type, object), Func<IServiceProvider, object>> _keyedScopedFactories;

    // Enumerable delegates
    private readonly Dictionary<Type, Func<IServiceProvider, object>> _enumerableFactories;

    // Open generics (oh, fuck 'em)
    private readonly List<ServiceDescriptor> _openGenericDescriptors;
    private readonly ConcurrentDictionary<Type, CompiledService> _openGenericClosedCache = new();
    
    private readonly Lock _lazySingletonLock = new();
    private int _nextSlotId;

    // Dispose tracking (all of that just works, just fucking believe, and don't ask questions)
    private readonly List<IDisposable> _disposables = [];
    private bool _disposed;

    internal ServiceProvider(
        Dictionary<(Type, object?), List<CompiledService>> compiled,
        List<ServiceDescriptor> openGenericDescriptors,
        List<CompiledService> singletonOrder,
        Dictionary<Type, Func<IServiceProvider, object>> enumerableFactories,
        int nextSlotId)
    {
        _nextSlotId = nextSlotId;
        _openGenericDescriptors = openGenericDescriptors;
        _enumerableFactories = enumerableFactories;

        _singletonCache = new Dictionary<Type, object>();
        _keyedSingletonCache = new Dictionary<(Type, object), object>();
        _transientFactories = new Dictionary<Type, Func<IServiceProvider, object>>();
        _keyedTransientFactories = new Dictionary<(Type, object), Func<IServiceProvider, object>>();
        _scopedFactories = new Dictionary<Type, Func<IServiceProvider, object>>();
        _keyedScopedFactories = new Dictionary<(Type, object), Func<IServiceProvider, object>>();

        // scoped and transient only
        foreach (var ((serviceType, key), services) in compiled)
        {
            var last = services[^1];

            switch (last.Lifetime)
            {
                case ServiceLifetimeType.Scoped:
                    if (key is null) _scopedFactories[serviceType] = last.Factory;
                    else _keyedScopedFactories[(serviceType, key)] = last.Factory;
                    break;

                case ServiceLifetimeType.Transient:
                    if (key is null) _transientFactories[serviceType] = last.Factory;
                    else _keyedTransientFactories[(serviceType, key)] = last.Factory;
                    break;
                // case ServiceLifetimeType.Singleton:
                // GetTheFuckOutOfThere();
                // break;
            }
        }

        // magic here
        foreach (var svc in singletonOrder)
        {
            var instance = svc.Factory(this);
            
            _singletonBySlot[svc.SlotId] = instance;
            
            if (svc.Key is null)
                _singletonCache[svc.ServiceType] = instance;
            else
                _keyedSingletonCache[(svc.ServiceType, svc.Key)] = instance;

            if (instance is IDisposable disposable)
                _disposables.Add(disposable);
        }
    }
    
    internal object ResolveFromCompiledService(CompiledService svc)
    {
        switch (svc.Lifetime)
        {
            case ServiceLifetimeType.Singleton:
                if (_singletonBySlot.TryGetValue(svc.SlotId, out var existing))
                    return existing;

                // lazy loading for open generics singleton after Build
                lock (_lazySingletonLock)
                {
                    if (_singletonBySlot.TryGetValue(svc.SlotId, out existing))
                        return existing;

                    var instance = svc.Factory(this);
                    _singletonBySlot[svc.SlotId] = instance;
                    if (instance is IDisposable d) _disposables.Add(d);
                    return instance;
                }

            case ServiceLifetimeType.Scoped:
                throw new InvalidOperationException(
                    $"Cannot resolve scoped service '{svc.ServiceType}' from root provider.");

            case ServiceLifetimeType.Transient:
                var t = svc.Factory(this);
                if (t is IDisposable td) _disposables.Add(td);
                return t;

            default:
                throw new InvalidOperationException();
        }
    }
    
    public object? GetService(Type serviceType)
    {
        ThrowIfDisposed();

        // Singleton?
        if (_singletonCache.TryGetValue(serviceType, out var singleton))
            return singleton;

        // Transient?
        if (_transientFactories.TryGetValue(serviceType, out var transientFactory))
            return CreateAndTrack(transientFactory);

        // Scoped? (exception)
        if (_scopedFactories.ContainsKey(serviceType))
            throw new InvalidOperationException(
                $"Cannot resolve scoped service '{serviceType}' from root provider. " +
                "Create a scope first.");

        // IEnumerable?
        if (serviceType.IsGenericType &&
            serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            if (_enumerableFactories.TryGetValue(serviceType, out var enumFactory))
                return enumFactory(this);
            
            var elementType = serviceType.GetGenericArguments()[0];
            return Array.CreateInstance(elementType, 0);
        }

        // Open generics?
        if (serviceType is { IsGenericType: true, IsGenericTypeDefinition: false })
        {
            var closedSvc = ResolveOpenGenericCompiled(serviceType);
            return ResolveFromCompiledService(closedSvc);
        }

        // Сосал?
        return null;
    }

    public object? GetKeyedService(Type serviceType, object key)
    {
        ThrowIfDisposed();

        if (_keyedSingletonCache.TryGetValue((serviceType, key), out var singleton))
            return singleton;

        if (_keyedTransientFactories.TryGetValue((serviceType, key), out var transientFactory))
            return CreateAndTrack(transientFactory);

        if (_keyedScopedFactories.ContainsKey((serviceType, key)))
            throw new InvalidOperationException(
                $"Cannot resolve keyed scoped service '{serviceType}' " +
                $"(key: '{key}') from root provider.");

        return null;
    }

    public IKeyedServiceProvider CreateScope()
    {
        ThrowIfDisposed();
        return new ServiceScope(this);
    }
    
    internal CompiledService ResolveOpenGenericCompiled(Type closedType)
    {
        var svc = _openGenericClosedCache.GetOrAdd(closedType, type =>
        {
            var genericDef = type.GetGenericTypeDefinition();
            var descriptor = _openGenericDescriptors
                .FirstOrDefault(d => d.ServiceType == genericDef);

            if (descriptor?.ImplementationType is null)
                return null!;

            var closedImpl = descriptor.ImplementationType
                .MakeGenericType(type.GetGenericArguments());

            var ctor = ReflectionHelper.GetSinglePublicConstructor(closedImpl);
            var parameters = ctor.GetParameters();

            var depTypes = new Type[parameters.Length];
            var depKeys = new object?[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var keyAttr = parameters[i].GetCustomAttribute<FromKeyedServicesAttribute>();
                depTypes[i] = ReflectionHelper.UnwrapEnumerable(parameters[i].ParameterType);
                depKeys[i] = keyAttr?.Key;
            }

            return new CompiledService
            {
                SlotId = Interlocked.Increment(ref _nextSlotId) - 1,
                ServiceType = type,
                ImplementationType = closedImpl,
                Lifetime = descriptor.Lifetime,
                Key = null,
                Factory = ExpressionFactoryCompiler.Compile(ctor, parameters),
                DependencyTypes = depTypes,
                DependencyKeys = depKeys
            };
        });
        return svc;
    }

    private object CreateAndTrack(Func<IServiceProvider, object> factory)
    {
        var instance = factory(this);
        if (instance is IDisposable disposable)
            _disposables.Add(disposable);
        return instance;
    }

    private void ThrowIfDisposed()
    {
        if (!_disposed) return;
        throw new ObjectDisposedException(nameof(ServiceProvider));
    }

    internal object? TryGetSingleton(Type serviceType) =>
        _singletonCache.GetValueOrDefault(serviceType);

    internal object? TryGetKeyedSingleton(Type serviceType, object key) =>
        _keyedSingletonCache.GetValueOrDefault((serviceType, key));

    internal bool HasScopedFactory(Type type) =>
        _scopedFactories.ContainsKey(type);

    internal Func<IServiceProvider, object> GetScopedFactory(Type type) =>
        _scopedFactories[type];

    internal bool HasTransientFactory(Type type) =>
        _transientFactories.ContainsKey(type);

    internal Func<IServiceProvider, object> GetTransientFactory(Type type) =>
        _transientFactories[type];

    internal bool HasKeyedScopedFactory(Type type, object key) =>
        _keyedScopedFactories.ContainsKey((type, key));

    internal Func<IServiceProvider, object> GetKeyedScopedFactory(Type type, object key) =>
        _keyedScopedFactories[(type, key)];

    internal bool HasKeyedTransientFactory(Type type, object key) =>
        _keyedTransientFactories.ContainsKey((type, key));

    internal Func<IServiceProvider, object> GetKeyedTransientFactory(Type type, object key) =>
        _keyedTransientFactories[(type, key)];

    internal bool HasEnumerableFactory(Type type) =>
        _enumerableFactories.ContainsKey(type);

    internal Func<IServiceProvider, object> GetEnumerableFactory(Type type) =>
        _enumerableFactories[type];

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        
        // inverted order disposing
        for (int i = _disposables.Count - 1; i >= 0; i--)
        {
            try { _disposables[i].Dispose(); }
            catch { /* хрюкни и сглотни */ }
        }

        _disposables.Clear();
        _singletonCache.Clear();
        _keyedSingletonCache.Clear();
    }
}