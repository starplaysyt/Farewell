using Farewell.DI;

namespace Farewell.Debug.Tests.DI;

public class ScopedTests
{
    [Fact]
    public void Scoped_SameInstanceWithinScope()
    {
        var provider = new ServiceBuilder()
            .AddScoped<IServiceA, ServiceA>()
            .Build();

        using var scope = provider.CreateScope();
        var a1 = scope.GetService<IServiceA>();
        var a2 = scope.GetService<IServiceA>();
        Assert.Same(a1, a2);
    }

    [Fact]
    public void Scoped_DifferentInstancesAcrossScopes()
    {
        var provider = new ServiceBuilder()
            .AddScoped<IServiceA, ServiceA>()
            .Build();

        IServiceA a1, a2;
        using (var scope1 = provider.CreateScope()) a1 = scope1.GetService<IServiceA>()!;
        using (var scope2 = provider.CreateScope()) a2 = scope2.GetService<IServiceA>()!;

        Assert.NotSame(a1, a2);
    }

    [Fact]
    public void Scoped_FromRootProvider_Throws()
    {
        var provider = new ServiceBuilder()
            .AddScoped<IServiceA, ServiceA>()
            .Build();

        Assert.Throws<InvalidOperationException>(
            provider.GetService<IServiceA>);
    }

    [Fact]
    public void Scoped_DisposedWithScope()
    {
        var provider = new ServiceBuilder()
            .AddScoped<DisposableService>()
            .Build();

        DisposableService svc;
        using (var scope = provider.CreateScope())
        {
            svc = scope.GetService<DisposableService>()!;
            Assert.False(svc.IsDisposed);
        }

        Assert.True(svc.IsDisposed);
    }

    [Fact]
    public void Scoped_NotDisposedWithOtherScope()
    {
        var provider = new ServiceBuilder()
            .AddScoped<DisposableService>()
            .Build();

        var scope1 = provider.CreateScope();
        var svc1 = scope1.GetService<DisposableService>()!;

        var scope2 = provider.CreateScope();
        var svc2 = scope2.GetService<DisposableService>()!;

        scope1.Dispose();
        Assert.True(svc1.IsDisposed);
        Assert.False(svc2.IsDisposed);

        scope2.Dispose();
        Assert.True(svc2.IsDisposed);
    }

    [Fact]
    public void Scoped_CanConsumeSingleton()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA, ServiceA>()
            .AddScoped<ServiceWithDependency>()
            .Build();

        var singleton = provider.GetService<IServiceA>();
        using var scope = provider.CreateScope();
        var scoped = scope.GetService<ServiceWithDependency>();

        Assert.Same(singleton, scoped!.A);
    }
}