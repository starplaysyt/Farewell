using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.DI;

namespace Farewell.Debug.Tests.DI;

public class SingletonTests
{
    [Fact]
    public void Singleton_ReturnsSameInstance_AcrossCalls()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA, ServiceA>()
            .Build();

        var a1 = provider.GetService<IServiceA>();
        var a2 = provider.GetService<IServiceA>();
        Assert.Same(a1, a2);
    }

    [Fact]
    public void Singleton_ReturnsSameInstance_AcrossScopes()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA, ServiceA>()
            .Build();

        IServiceA a1, a2;
        using (var scope1 = provider.CreateScope()) a1 = scope1.GetService<IServiceA>()!;
        using (var scope2 = provider.CreateScope()) a2 = scope2.GetService<IServiceA>()!;

        Assert.Same(a1, a2);
    }

    [Fact]
    public void Singleton_CreatedEagerly_OnBuild()
    {
        var creationCount = 0;
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA>(_ =>
            {
                creationCount++;
                return new ServiceA();
            })
            .Build();

        Assert.Equal(1, creationCount);

        provider.GetService<IServiceA>();
        provider.GetService<IServiceA>();
        Assert.Equal(1, creationCount);
    }

    [Fact]
    public void Singleton_WithDependency_Resolves()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA, ServiceA>()
            .AddSingleton<ServiceWithDependency>()
            .Build();

        var s = provider.GetService<ServiceWithDependency>();
        Assert.NotNull(s);
        Assert.NotNull(s.A);
    }

    [Fact]
    public void Singleton_DisposedWithProvider()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<DisposableService>()
            .Build();

        var svc = provider.GetService<DisposableService>()!;
        Assert.False(svc.IsDisposed);

        provider.Dispose();
        Assert.True(svc.IsDisposed);
    }
}