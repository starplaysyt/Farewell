using Farewell.Abstractions.DI;
using Farewell.DI;

namespace Farewell.Debug.Tests.DI;

public class KeyedTests
{
    [Fact]
    public void KeyedSingleton_ResolvedByKey()
    {
        var provider = new ServiceBuilder()
            .AddKeyedSingleton<ICache, MemoryCache>("fast")
            .AddKeyedSingleton<ICache, RedisCache>("distributed")
            .Build();

        var fast = provider.GetRequiredKeyedService<ICache>("fast");
        var slow = provider.GetRequiredKeyedService<ICache>("distributed");

        Assert.IsType<MemoryCache>(fast);
        Assert.IsType<RedisCache>(slow);
    }

    [Fact]
    public void KeyedService_WrongKey_ReturnsNull()
    {
        var provider = new ServiceBuilder()
            .AddKeyedSingleton<ICache, MemoryCache>("fast")
            .Build();

        var result = provider.GetKeyedService<ICache>("nonexistent");
        Assert.Null(result);
    }

    [Fact]
    public void KeyedService_NonKeyedGetService_ReturnsNull()
    {
        var provider = new ServiceBuilder()
            .AddKeyedSingleton<ICache, MemoryCache>("fast")
            .Build();

        var result = provider.GetService<ICache>();
        Assert.Null(result);
    }

    [Fact]
    public void KeyedScoped_SameInstanceWithinScope()
    {
        var provider = new ServiceBuilder()
            .AddKeyedScoped<ICache, MemoryCache>("s")
            .Build();

        using var scope = provider.CreateScope();
        var c1 = scope.GetRequiredKeyedService<ICache>("s");
        var c2 = scope.GetRequiredKeyedService<ICache>("s");
        Assert.Same(c1, c2);
    }

    [Fact]
    public void KeyedTransient_NewInstance()
    {
        var provider = new ServiceBuilder()
            .AddKeyedTransient<ICache, MemoryCache>("t")
            .Build();

        var c1 = provider.GetRequiredKeyedService<ICache>("t");
        var c2 = provider.GetRequiredKeyedService<ICache>("t");
        Assert.NotSame(c1, c2);
    }

    [Fact]
    public void KeyedService_ViaAttribute_InjectedCorrectly()
    {
        var provider = new ServiceBuilder()
            .AddKeyedSingleton<ICache, MemoryCache>("fast")
            .AddKeyedSingleton<ICache, RedisCache>("distributed")
            .AddSingleton<KeyedConsumer>()
            .Build();

        var consumer = provider.GetService<KeyedConsumer>()!;
        Assert.IsType<MemoryCache>(consumer.Fast);
        Assert.IsType<RedisCache>(consumer.Distributed);
    }

    [Fact]
    public void KeyedService_ViaFactory()
    {
        var instance = new MemoryCache();
        var provider = new ServiceBuilder()
            .AddKeyedSingleton<ICache>("fast", _ => instance)
            .Build();

        var resolved = provider.GetRequiredKeyedService<ICache>("fast");
        Assert.Same(instance, resolved);
    }

    [Fact]
    public void GetRequiredKeyedService_NotRegistered_Throws()
    {
        var provider = new ServiceBuilder().Build();
        Assert.Throws<InvalidOperationException>(() =>
            provider.GetRequiredKeyedService<ICache>("any"));
    }
}