using Farewell.DI;

namespace Farewell.Debug.Tests.DI;

public class DisposeTests
{
    [Fact]
    public void Provider_Disposed_GetServiceThrows()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA, ServiceA>()
            .Build();

        provider.Dispose();
        Assert.Throws<ObjectDisposedException>(provider.GetService<IServiceA>);
    }

    [Fact]
    public void Scope_Disposed_GetServiceThrows()
    {
        var provider = new ServiceBuilder()
            .AddScoped<IServiceA, ServiceA>()
            .Build();

        var scope = provider.CreateScope();
        scope.Dispose();

        Assert.Throws<ObjectDisposedException>(scope.GetService<IServiceA>);
    }

    [Fact]
    public void Provider_DoubleDispose_DoesNotThrow()
    {
        var provider = new ServiceBuilder().Build();
        provider.Dispose();
        provider.Dispose(); // no throw
    }

    [Fact]
    public void Scope_DoubleDispose_DoesNotThrow()
    {
        var provider = new ServiceBuilder().Build();
        var scope = provider.CreateScope();
        scope.Dispose();
        scope.Dispose();
    }

    [Fact]
    public void MultipleDisposables_AllDisposed()
    {
        var provider = new ServiceBuilder()
            .AddScoped<DisposableService>()
            .AddScoped<DisposableServiceWithDep>()
            .AddScoped<IServiceA, ServiceA>()
            .Build();

        DisposableService d1;
        DisposableServiceWithDep d2;
        using (var scope = provider.CreateScope())
        {
            d1 = scope.GetService<DisposableService>()!;
            d2 = scope.GetService<DisposableServiceWithDep>()!;
        }

        Assert.True(d1.IsDisposed);
        Assert.True(d2.IsDisposed);
    }

    [Fact]
    public void ProviderDispose_DisposesSingletons()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<DisposableService>()
            .Build();

        var svc = provider.GetService<DisposableService>()!;
        provider.Dispose();
        Assert.True(svc.IsDisposed);
    }
}