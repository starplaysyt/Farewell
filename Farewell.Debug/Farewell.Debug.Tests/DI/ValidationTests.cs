using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.DI;
using Farewell.DI.Exceptions;

namespace Farewell.Debug.Tests.DI;

public class ValidationTests
{
    [Fact]
    public void Build_UnregisteredDependency_Throws()
    {
        var builder = new ServiceBuilder()
            .AddSingleton<ServiceWithDependency>();

        Assert.Throws<DependencyValidationException>(builder.Build);
    }

    [Fact]
    public void Build_CircularDependency_Throws()
    {
        var builder = new ServiceBuilder()
            .AddSingleton<CycleA>()
            .AddSingleton<CycleB>();

        Assert.Throws<DependencyValidationException>(builder.Build);
    }

    [Fact]
    public void Build_SelfDependency_Throws()
    {
        var builder = new ServiceBuilder()
            .AddSingleton<SelfCycle>();

        Assert.Throws<DependencyValidationException>(builder.Build);
    }

    [Fact]
    public void Build_SingletonDependsOnScoped_Throws()
    {
        var builder = new ServiceBuilder()
            .AddScoped<IScopedDep, ScopedDep>()
            .AddSingleton<SingletonWithScoped>();

        Assert.Throws<DependencyValidationException>(builder.Build);
    }

    [Fact]
    public void Build_MultipleConstructors_Throws()
    {
        var builder = new ServiceBuilder()
            .AddSingleton<IServiceA, ServiceA>()
            .AddSingleton<MultipleConstructors>();

        Assert.Throws<MultiplePublicConstructorsException>(builder.Build);
    }

    [Fact]
    public void Build_NoPublicConstructor_Throws()
    {
        var builder = new ServiceBuilder()
            .AddSingleton<NoPublicConstructor>();

        Assert.Throws<NoPublicConstructorsException>(builder.Build);
    }

    [Fact]
    public void AddSingleton_ImplementationNotAssignable_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceDescriptor.FromType(
                typeof(IServiceA), typeof(ServiceB), ServiceLifetimeType.Singleton));
    }

    [Fact]
    public void AddSingleton_AbstractImplementation_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            ServiceDescriptor.FromType(
                typeof(IServiceA), typeof(AbstractService), ServiceLifetimeType.Singleton));
    }

    public abstract class AbstractService : IServiceA
    {
        public string Id => "abstract";
    }
}