using Farewell.Abstractions.Attributes.Validation;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Validation;
using Farewell.DI;
using Farewell.Validation;
using Farewell.Validation.Extensions;

namespace Farewell.Debug.Tests.Validation;

public class User
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int Age { get; set; }
    public string? Phone { get; set; }
    public bool HasPhone { get; set; }
}

public class Rank
{
    public string? Name { get; set; }
    public string? ShortName { get; set; }
}

public class UnvalidatedEntity
{
    public string? Something { get; set; }
}

public interface IAppConfig
{
    int MinNameLength { get; }
    int MaxNameLength { get; }
}

public sealed class TestAppConfig : IAppConfig
{
    public int MinNameLength { get; set; } = 2;
    public int MaxNameLength { get; set; } = 50;
}

public interface IRankRepository
{
    Task<bool> ExistsByNameAsync(string? name, CancellationToken ct);
    Task<bool> ExistsByShortNameAsync(string? shortName, CancellationToken ct);
    void Seed(string name, string shortName);
}

public sealed class InMemoryRankRepository : IRankRepository
{
    private readonly HashSet<string> _names = new();
    private readonly HashSet<string> _shortNames = new();

    public Task<bool> ExistsByNameAsync(string? name, CancellationToken ct)
        => Task.FromResult(name is not null && _names.Contains(name));

    public Task<bool> ExistsByShortNameAsync(string? shortName, CancellationToken ct)
        => Task.FromResult(shortName is not null && _shortNames.Contains(shortName));

    public void Seed(string name, string shortName)
    {
        _names.Add(name);
        _shortNames.Add(shortName);
    }
}

public sealed class UserValidator : FluentValidator<User>
{
    protected override void DefineRules(PropertiesValidatorBuilder<User> builder)
    {
        builder.Rule(u => u.Name)
            .NotEmpty()
            .Length(2, 50);

        builder.Rule(u => u.Email)
            .NotEmpty()
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        builder.Rule(u => u.Age)
            .InRange(0, 150);

        builder.Rule(u => u.Phone)
            .When(u => u.HasPhone, r => r
                .NotEmpty()
                .Matches(@"^\+7\d{10}$"));
    }
}

[ValidationContext("Create")]
public sealed class UserCreateValidator : FluentValidator<User>
{
    protected override void DefineRules(PropertiesValidatorBuilder<User> builder)
    {
        builder.Rule(u => u.Name)
            .NotEmpty()
            .MinLength(5);
    }
}

public class UserDynamicValidator : FluentValidator<User>
{
    protected override void DefineRules(PropertiesValidatorBuilder<User> builder)
    {
        builder.Rule(u => u.Name)
            .NotEmpty()
            .MinLength(sp => ServiceProviderExtensions.GetRequiredService<IAppConfig>(sp).MinNameLength)
            .MaxLength(sp => ServiceProviderExtensions.GetRequiredService<IAppConfig>(sp).MaxNameLength);
    }
}

[ValidationContext("Dynamic")]
public sealed class UserDynamicValidatorKeyed : UserDynamicValidator { }

public sealed class RankUniqueValidator(IRankRepository repository) : IDirectValidator<Rank>
{
    public async ValueTask<ValidationReport> ValidateAsync(
        Rank instance,
        IServiceProvider sp,
        IValidationCollector collector,
        CancellationToken ct = default)
    {
        if (await repository.ExistsByNameAsync(instance.Name, ct).ConfigureAwait(false))
            collector.Collect(ValidationStatus.Error(ValidationCode.NotUnique, nameof(Rank.Name)));

        if (await repository.ExistsByShortNameAsync(instance.ShortName, ct).ConfigureAwait(false))
            collector.Collect(ValidationStatus.Error(ValidationCode.NotUnique, nameof(Rank.ShortName)));

        return collector.ToReport();
    }
}

public static class ProviderFactory
{
    public static IScopeProvider BuildServices(Action<ServiceBuilder>? configure = null)
    {
        var services = new ServiceBuilder();

        services.AddValidation();
        
        services.AddValidator<UserValidator>();
        services.AddValidator<UserCreateValidator>("Create");
        services.AddValidator<UserDynamicValidatorKeyed>("Dynamic");
        services.AddValidator<RankUniqueValidator>();
        
        services.AddSingleton<IAppConfig, TestAppConfig>();
        services.AddScoped<IRankRepository, InMemoryRankRepository>();

        configure?.Invoke(services);

        return services.Build();
    }
    
    public static (IKeyedServiceProvider scope, IValidationProvider provider) CreateProvider(
        Action<IServiceBuilder>? configure = null)
    {
        var root = BuildServices(configure);
        var scope = root.CreateScope();
        var provider = scope.GetRequiredService<IValidationProvider>();
        return (scope, provider);
    }
}