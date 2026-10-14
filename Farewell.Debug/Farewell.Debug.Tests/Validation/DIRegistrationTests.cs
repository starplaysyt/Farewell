using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Validation;
using Farewell.DI;
using Farewell.Validation.Extensions;

namespace Farewell.Debug.Tests.Validation;

public sealed class DIRegistrationTests
{
    [Fact]
    public void AddValidator_RegistersFluentAsSingleton()
    {
        var services = new ServiceBuilder();
        services.AddValidation();
        services.AddValidator<UserValidator>();
        var sp = services.Build();

        var v1 = sp.GetRequiredKeyedService<IAsyncValidator<User>>("Default");
        var v2 = sp.GetRequiredKeyedService<IAsyncValidator<User>>("Default");

        Assert.Same(v1, v2);
    }

    [Fact]
    public void AddValidator_RegistersDirectAsScoped()
    {
        var services = new ServiceBuilder();
        services.AddValidation();
        services.AddValidator<RankUniqueValidator>();
        services.AddScoped<IRankRepository, InMemoryRankRepository>();
        var sp = services.Build();

        IAsyncValidator<Rank> v1, v2;

        using (var scope = sp.CreateScope())
            v1 = scope.GetRequiredKeyedService<IAsyncValidator<Rank>>("Default");

        using (var scope = sp.CreateScope())
            v2 = scope.GetRequiredKeyedService<IAsyncValidator<Rank>>("Default");

        Assert.NotSame(v1, v2);
    }

    [Fact]
    public void AddValidator_WithContext_RegistersUnderKey()
    {
        var services = new ServiceBuilder();
        services.AddValidation();
        services.AddValidator<UserCreateValidator>("Create");
        var sp = services.Build();

        var validator = sp.GetRequiredKeyedService<IAsyncValidator<User>>("Create");
        Assert.NotNull(validator);
    }

    [Fact]
    public void AddValidator_ContextAttributeIsUsed()
    {
        var services = new ServiceBuilder();
        services.AddSingleton<IRankRepository, InMemoryRankRepository>();
        services.AddValidation();
        services.AddValidators(typeof(UserCreateValidator).Assembly);
        var sp = services.Build();

        var validator = sp.GetRequiredKeyedService<IAsyncValidator<User>>("Create");
        Assert.NotNull(validator);
        Assert.IsType<UserCreateValidator>(validator);
    }

    [Fact]
    public void Provider_IsScoped()
    {
        var sp = ProviderFactory.BuildServices();

        IValidationProvider p1, p2;

        using (var scope = sp.CreateScope())
            p1 = scope.GetRequiredService<IValidationProvider>();

        using (var scope = sp.CreateScope())
            p2 = scope.GetRequiredService<IValidationProvider>();

        Assert.NotSame(p1, p2);
    }
}