using Farewell.Abstractions.DI;
using Farewell.DI;

namespace Farewell.Debug.Tests.DI;

public class TransientTests
{
    [Fact]
    public void Transient_NewInstanceEveryCall()
    {
        var provider = new ServiceBuilder()
            .AddTransient<IServiceA, ServiceA>()
            .Build();

        var a1 = provider.GetService<IServiceA>();
        var a2 = provider.GetService<IServiceA>();
        Assert.NotSame(a1, a2);
    }

    [Fact]
    public void Transient_FromProvider_TrackedByProvider()
    {
        DisposableService svc;
        var provider = new ServiceBuilder()
            .AddTransient<DisposableService>()
            .Build();

        svc = provider.GetService<DisposableService>()!;
        Assert.False(svc.IsDisposed);

        provider.Dispose();
        Assert.True(svc.IsDisposed);
    }

    [Fact]
    public void Transient_FromScope_TrackedByScope()
    {
        var provider = new ServiceBuilder()
            .AddTransient<DisposableService>()
            .Build();

        DisposableService svc;
        using (var scope = provider.CreateScope())
        {
            svc = scope.GetService<DisposableService>()!;
        }

        Assert.True(svc.IsDisposed);
    }

    [Fact]
    public void Transient_CanConsumeSingleton()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA, ServiceA>()
            .AddTransient<ServiceWithDependency>()
            .Build();

        var singleton = provider.GetService<IServiceA>();
        var t1 = provider.GetService<ServiceWithDependency>();
        var t2 = provider.GetService<ServiceWithDependency>();

        Assert.NotSame(t1, t2);
        Assert.Same(singleton, t1!.A);
        Assert.Same(singleton, t2!.A);
    }
}