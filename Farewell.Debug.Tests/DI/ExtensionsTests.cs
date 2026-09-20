using Farewell.Abstractions.DI;
using Farewell.DI;

namespace Farewell.Debug.Tests.DI;

public class ExtensionsTests
{
    [Fact]
    public void GetService_Generic_ReturnsTypedInstance()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA, ServiceA>()
            .Build();

        IServiceA? a = provider.GetService<IServiceA>();
        Assert.NotNull(a);
    }

    [Fact]
    public void GetRequiredService_Generic_ThrowsIfMissing()
    {
        var provider = new ServiceBuilder().Build();
        Assert.Throws<InvalidOperationException>(
            provider.GetRequiredService<IServiceA>);
    }

    [Fact]
    public void GetRequiredService_NonGeneric_Works()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA, ServiceA>()
            .Build();

        var svc = provider.GetRequiredService(typeof(IServiceA));
        Assert.IsAssignableFrom<IServiceA>(svc);
    }

    [Fact]
    public void GetServices_NonGeneric_Works()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<INotifier, EmailNotifier>()
            .AddSingleton<INotifier, SmsNotifier>()
            .Build();

        var services = provider.GetServices(typeof(INotifier)).ToList();
        Assert.Equal(2, services.Count);
    }

    [Fact]
    public void GetKeyedService_Generic_ReturnsTypedInstance()
    {
        var provider = new ServiceBuilder()
            .AddKeyedSingleton<ICache, MemoryCache>("k")
            .Build();

        var cache = provider.GetKeyedService<ICache>("k");
        Assert.NotNull(cache);
    }

    [Fact]
    public void GetService_NullProvider_Throws()
    {
        IServiceProvider provider = null!;
        Assert.Throws<ArgumentNullException>(provider.GetService<IServiceA>);
    }

    [Fact]
    public void GetKeyedService_NullKey_Throws()
    {
        var provider = new ServiceBuilder().Build();
        Assert.Throws<ArgumentNullException>(() => provider.GetKeyedService<ICache>(null!));
    }
}