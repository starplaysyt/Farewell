using Farewell.DI;

namespace Farewell.Debug.Tests.DI;

public interface IServiceA
{
    string Id { get; }
}

public interface IServiceB
{
    string Id { get; }
}

public interface IServiceC
{
    string Id { get; }
}

public class ServiceA : IServiceA
{
    public string Id { get; } = Guid.NewGuid().ToString();
}

public class ServiceB : IServiceB
{
    public string Id { get; } = Guid.NewGuid().ToString();
}

public class ServiceC : IServiceC
{
    public string Id { get; } = Guid.NewGuid().ToString();
}

public class ServiceWithDependency
{
    public IServiceA A { get; }
    public ServiceWithDependency(IServiceA a) => A = a;
}

public class ServiceWithTwoDependencies
{
    public IServiceA A { get; }
    public IServiceB B { get; }

    public ServiceWithTwoDependencies(IServiceA a, IServiceB b)
    {
        A = a;
        B = b;
    }
}

public class DisposableService : IDisposable
{
    public bool IsDisposed { get; private set; }
    public void Dispose() => IsDisposed = true;
}

public class DisposableServiceWithDep : IDisposable
{
    public IServiceA A { get; }
    public bool IsDisposed { get; private set; }
    public DisposableServiceWithDep(IServiceA a) => A = a;
    public void Dispose() => IsDisposed = true;
}

public interface INotifier
{
    string Name { get; }
}

public class EmailNotifier : INotifier
{
    public string Name => "email";
}

public class SmsNotifier : INotifier
{
    public string Name => "sms";
}

public class PushNotifier : INotifier
{
    public string Name => "push";
}

public class NotifierConsumer(IEnumerable<INotifier> notifiers)
{
    public IEnumerable<INotifier> Notifiers { get; } = notifiers;
}

public class CycleA
{
    public CycleA(CycleB b)
    {
    }
}

public class CycleB
{
    public CycleB(CycleA a)
    {
    }
}

public class SelfCycle
{
    public SelfCycle(SelfCycle self)
    {
    }
}

public interface IScopedDep
{
}

public class ScopedDep : IScopedDep
{
}

public class SingletonWithScoped
{
    public SingletonWithScoped(IScopedDep dep)
    {
    }
}

public class MultipleConstructors
{
    public MultipleConstructors()
    {
    }

    public MultipleConstructors(IServiceA a)
    {
    }
}

public class NoPublicConstructor
{
    private NoPublicConstructor()
    {
    }
}

public interface IRepository<T>
{
    Type EntityType { get; }
}

public class Repository<T> : IRepository<T>
{
    public Type EntityType => typeof(T);
}

public class User
{
}

public class Order
{
}

public class RepositoryConsumer(IRepository<User> userRepo)
{
    public IRepository<User> UserRepo { get; } = userRepo;
}

public interface ICache
{
    string Name { get; }
}

public class MemoryCache : ICache
{
    public string Name => "memory";
}

public class RedisCache : ICache
{
    public string Name => "redis";
}

public class KeyedConsumer(
    [FromKeyedServices("fast")]
    ICache fast,
    [FromKeyedServices("distributed")]
    ICache distributed)
{
    public ICache Fast { get; } = fast;
    public ICache Distributed { get; } = distributed;
}