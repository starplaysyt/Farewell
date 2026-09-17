using Farewell.Abstractions.DI;
using Farewell.DI;

namespace Farewell.Debug.Tests.DI;

public class OpenGenericTests
{
    [Fact]
    public void OpenGeneric_ClosedResolve_Works()
    {
        var provider = new ServiceBuilder()
            .AddSingleton(typeof(IRepository<>), typeof(Repository<>))
            .Build();

        var repo = provider.GetService<IRepository<User>>();
        Assert.NotNull(repo);
        Assert.Equal(typeof(User), repo.EntityType);
    }

    [Fact]
    public void OpenGeneric_DifferentClosedTypes_DifferentInstances()
    {
        var provider = new ServiceBuilder()
            .AddSingleton(typeof(IRepository<>), typeof(Repository<>))
            .Build();

        var userRepo = provider.GetService<IRepository<User>>();
        var orderRepo = provider.GetService<IRepository<Order>>();

        Assert.Equal(typeof(User), userRepo!.EntityType);
        Assert.Equal(typeof(Order), orderRepo!.EntityType);
    }

    [Fact]
    public void OpenGeneric_InjectedIntoConstructor()
    {
        var provider = new ServiceBuilder()
            .AddSingleton(typeof(IRepository<>), typeof(Repository<>))
            .AddSingleton<RepositoryConsumer>()
            .Build();

        var consumer = provider.GetService<RepositoryConsumer>()!;
        Assert.NotNull(consumer.UserRepo);
        Assert.Equal(typeof(User), consumer.UserRepo.EntityType);
    }

    [Fact]
    public void OpenGeneric_LazyResolve_CachedOnSecondCall()
    {
        var provider = new ServiceBuilder()
            .AddTransient(typeof(IRepository<>), typeof(Repository<>))
            .Build();

        var r1 = provider.GetService<IRepository<User>>();
        var r2 = provider.GetService<IRepository<User>>();

        Assert.NotNull(r1);
        Assert.NotNull(r2);
        // Смотри что это? И ОНО ЗАКЕШИРОВАНО!
        Assert.NotSame(r1, r2);
    }

    [Fact]
    public void OpenGeneric_Scoped_CachedWithinScope()
    {
        var provider = new ServiceBuilder()
            .AddScoped(typeof(IRepository<>), typeof(Repository<>))
            .Build();

        using var scope = provider.CreateScope();
        var r1 = scope.GetService<IRepository<User>>();
        var r2 = scope.GetService<IRepository<User>>();
        Assert.Same(r1, r2);
    }

    [Fact]
    public void OpenGeneric_Singleton_SameInstanceAcrossCalls()
    {
        var provider = new ServiceBuilder()
            .AddSingleton(typeof(IRepository<>), typeof(Repository<>))
            .Build();

        var r1 = provider.GetService<IRepository<User>>();
        var r2 = provider.GetService<IRepository<User>>();
        Assert.Same(r1, r2);
    }

    [Fact]
    public void OpenGeneric_ServiceIsOpenImplementationIsClosed_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceDescriptor.FromType(
                typeof(IRepository<>),
                typeof(Repository<User>),
                ServiceLifetimeType.Singleton));
    }

    [Fact]
    public void OpenGeneric_Factory_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceDescriptor.FromFactory(
                typeof(IRepository<>),
                _ => new Repository<User>(),
                ServiceLifetimeType.Singleton));
    }
}