using Farewell.DI;

namespace Farewell.Debug.Tests.DI;

public class RegistrationTests
{
    [Fact]
    public void AddSingleton_TServiceTImplementation_Resolves()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA, ServiceA>()
            .Build();

        var a = provider.GetService<IServiceA>();
        Assert.NotNull(a);
        Assert.IsType<ServiceA>(a);
    }

    [Fact]
    public void AddSingleton_ConcreteType_Resolves()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<ServiceA>()
            .Build();

        var a = provider.GetService<ServiceA>();
        Assert.NotNull(a);
    }

    [Fact]
    public void AddSingleton_Factory_UsesFactory()
    {
        var instance = new ServiceA();
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA>(_ => instance)
            .Build();

        var resolved = provider.GetService<IServiceA>();
        Assert.Same(instance, resolved);
    }

    [Fact]
    public void AddScoped_TypeMapping_Resolves()
    {
        var provider = new ServiceBuilder()
            .AddScoped<IServiceA, ServiceA>()
            .Build();

        using var scope = provider.CreateScope();
        var a = scope.GetService<IServiceA>();
        Assert.NotNull(a);
    }

    [Fact]
    public void AddTransient_TypeMapping_Resolves()
    {
        var provider = new ServiceBuilder()
            .AddTransient<IServiceA, ServiceA>()
            .Build();

        var a = provider.GetService<IServiceA>();
        Assert.NotNull(a);
    }

    [Fact]
    public void FluentApi_ChainsWork()
    {
        var provider = new ServiceBuilder()
            .AddSingleton<IServiceA, ServiceA>()
            .AddScoped<IServiceB, ServiceB>()
            .AddTransient<IServiceC, ServiceC>()
            .Build();

        Assert.NotNull(provider.GetService<IServiceA>());
        using var scope = provider.CreateScope();
        Assert.NotNull(scope.GetService<IServiceB>());
        Assert.NotNull(provider.GetService<IServiceC>());
    }

    [Fact]
    public void GetService_UnregisteredService_ReturnsNull()
    {
        var provider = new ServiceBuilder().Build();
        Assert.Null(provider.GetService<IServiceA>());
    }

    [Fact]
    public void GetRequiredService_UnregisteredService_Throws()
    {
        var provider = new ServiceBuilder().Build();
        Assert.Throws<InvalidOperationException>(
            provider.GetRequiredService<IServiceA>);
    }
}