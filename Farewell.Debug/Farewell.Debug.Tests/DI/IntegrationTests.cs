using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.DI;

namespace Farewell.Debug.Tests.DI;

public class IntegrationTests
{
    [Fact]
    public void DeepDependencyChain_Resolves()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA, ServiceA>()
            .AddSingleton<IServiceB, ServiceB>()
            .AddSingleton<ServiceWithTwoDependencies>()
            .Build();

        var svc = provider.GetService<ServiceWithTwoDependencies>();
        Assert.NotNull(svc);
        Assert.NotNull(svc!.A);
        Assert.NotNull(svc.B);
    }

    [Fact]
    public void MixedLifetimes_ScopeIsolation_Works()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA, ServiceA>()
            .AddScoped<IServiceB, ServiceB>()
            .AddTransient<IServiceC, ServiceC>()
            .Build();

        var singleton1 = provider.GetService<IServiceA>();
        var singleton2 = provider.GetService<IServiceA>();
        Assert.Same(singleton1, singleton2);

        IServiceB scoped1, scoped2;
        using (var scope1 = provider.CreateScope())
        {
            scoped1 = scope1.GetService<IServiceB>()!;
            Assert.Same(scoped1, scope1.GetService<IServiceB>());
        }

        using (var scope2 = provider.CreateScope())
        {
            scoped2 = scope2.GetService<IServiceB>()!;
        }

        Assert.NotSame(scoped1, scoped2);

        var trans1 = provider.GetService<IServiceC>();
        var trans2 = provider.GetService<IServiceC>();
        Assert.NotSame(trans1, trans2);
    }

    [Fact]
    public void LastRegistrationWins_ForSingleResolve()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<INotifier, EmailNotifier>()
            .AddSingleton<INotifier, SmsNotifier>()
            .Build();

        var single = provider.GetService<INotifier>();
        Assert.IsType<SmsNotifier>(single);
    }

    [Fact]
    public void ManyServices_LargeGraph_BuildsAndResolves()
    {
        var builder = new ServiceBuilder();
        builder.AddSingleton<IServiceA, ServiceA>();
        builder.AddSingleton<IServiceB, ServiceB>();
        builder.AddSingleton<IServiceC, ServiceC>();
        builder.AddSingleton<ServiceWithTwoDependencies>();
        builder.AddScoped<ServiceWithDependency>();
        builder.AddTransient<DisposableService>();
        builder.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
        builder.AddKeyedSingleton<ICache, MemoryCache>("fast");
        builder.AddKeyedSingleton<ICache, RedisCache>("slow");
        builder.AddSingleton<INotifier, EmailNotifier>();
        builder.AddSingleton<INotifier, SmsNotifier>();

        using var provider = builder.Build();

        Assert.NotNull(provider.GetService<ServiceWithTwoDependencies>());
        Assert.NotNull(provider.GetService<IRepository<User>>());
        Assert.Equal(2, provider.GetServices<INotifier>().Count());

        using var scope = provider.CreateScope();
        Assert.NotNull(scope.GetService<ServiceWithDependency>());
    }

    [Fact]
    public void NestedScopes_ScopedAreIndependent()
    {
        var provider = new ServiceBuilder()
            .AddScoped<IServiceA, ServiceA>()
            .Build();

        using var outer = provider.CreateScope();
        using var inner = provider.CreateScope();

        var a1 = outer.GetService<IServiceA>();
        var a2 = inner.GetService<IServiceA>();

        Assert.NotSame(a1, a2);
    }
}