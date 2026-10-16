using System.Reflection;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Validation;
using Farewell.DI;
using Farewell.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationDIRegistrationTests
{
    [Fact]
    public void AddValidation_RegistersProvider()
    {
        var services = new ServiceBuilder();
        services.AddValidation();
        using var sp = services.Build().CreateScope();

        var provider = sp.GetService<IValidationProvider>();
        Assert.NotNull(provider);
    }

    [Fact]
    public void AddValidation_RegistersPresenter()
    {
        var services = new ServiceBuilder();
        services.AddValidation();
        var sp = services.Build();

        var presenter = sp.GetService<IValidationPresenter>();
        Assert.NotNull(presenter);
    }

    [Fact]
    public void AddValidation_RegistersResolver()
    {
        var services = new ServiceBuilder();
        services.AddValidation();
        var sp = services.Build();

        var resolver = sp.GetService<IValidationMessageResolver>();
        Assert.NotNull(resolver);
    }

    [Fact]
    public void AddValidators_RegistersDefaultContext()
    {
        var services = new ServiceBuilder();
        services.AddValidation();
        services.AddValidators(Assembly.GetExecutingAssembly());
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        using var sp = services.Build().CreateScope();

        var validator = sp.GetKeyedService<IAsyncValidator<User>>("Default");
        Assert.NotNull(validator);
    }

    [Fact]
    public void AddValidators_RegistersStrictContext()
    {
        var services = new ServiceBuilder();
        services.AddValidation();
        services.AddValidators(Assembly.GetExecutingAssembly());
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        using var sp = services.Build().CreateScope();

        var validator = sp.GetKeyedService<IAsyncValidator<User>>("Strict");
        Assert.NotNull(validator);
    }

    [Fact]
    public void AddValidators_RegistersOrderValidator()
    {
        var services = new ServiceBuilder();
        services.AddValidation();
        services.AddValidators(Assembly.GetExecutingAssembly());
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        using var sp = services.Build().CreateScope();

        var validator = sp.GetKeyedService<IAsyncValidator<Order>>("Default");
        Assert.NotNull(validator);
    }

    [Fact]
    public void AddValidators_RegistersDirectValidator()
    {
        var services = new ServiceBuilder();
        services.AddValidation();
        services.AddSingleton<IUserRepository>((_) => new InMemoryUserRepository());
        services.AddValidators(Assembly.GetExecutingAssembly());
        var sp = services.Build();

        var scope = sp.CreateScope();
        var validator = scope
            .GetKeyedService<IAsyncValidator<User>>("Default");
        Assert.NotNull(validator);
    }

    [Fact]
    public void MissingContext_ThrowsFromDI()
    {
        var services = new ServiceBuilder();
        services.AddValidation();
        services.AddValidators(Assembly.GetExecutingAssembly());
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        using var sp = services.Build().CreateScope();

        Assert.Throws<InvalidOperationException>(() =>
            sp.GetRequiredKeyedService<IFluentValidator<User>>("NonExistent"));
    }

    [Fact]
    public void FluentValidator_IsSingleton()
    {
        var services = new ServiceBuilder();
        services.AddValidation();
        services.AddValidators(Assembly.GetExecutingAssembly());
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        using var sp = services.Build().CreateScope();

        var a = sp.GetKeyedService<IFluentValidator<User>>("Default");
        var b = sp.GetKeyedService<IFluentValidator<User>>("Default");
        Assert.Same(a, b);
    }
}