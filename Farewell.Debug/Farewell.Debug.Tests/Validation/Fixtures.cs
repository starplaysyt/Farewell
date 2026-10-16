using System.Reflection;
using Farewell.Abstractions.Attributes.Validation;
using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Validation;
using Farewell.DI;
using Farewell.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class User
{
    public string Name { get; set; } = "";
    public string? Email { get; set; }
    public int Age { get; set; }
    public bool IsPremium { get; set; }
    public string? CompanyName { get; set; }
    public decimal Salary { get; set; }
}

public sealed class Order
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = "";
}

public interface IUserRepository
{
    ValueTask<bool> EmailExistsAsync(string email, CancellationToken ct);
}

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly HashSet<string> _emails = new(StringComparer.OrdinalIgnoreCase);

    public void Add(string email) => _emails.Add(email);

    public ValueTask<bool> EmailExistsAsync(string email, CancellationToken ct)
        => new(_emails.Contains(email));
}

[ValidationContext("Default")]
public sealed class UserValidator : FluentValidator<User>
{
    protected override void DefineRules(IConstraintBuilder<User> b)
    {
        b.Property(x => x.Name)
            .NotEmpty()
            .MinLength(3)
            .MaxLength(50);

        b.Property(x => x.Email)
            .NotEmpty()
            .Email();

        b.Property(x => x.Age, x => x.Age > 0)
            .InRange(1, 150);

        b.Property(x => x.CompanyName, x => x.IsPremium)
            .NotEmpty()
            .MinLength(2);
    }
}

[ValidationContext("Strict")]
public sealed class StrictUserValidator : FluentValidator<User>
{
    protected override void DefineRules(IConstraintBuilder<User> b)
    {
        b.Property(x => x.Name)
            .NotEmpty()
            .MinLength(5)
            .MaxLength(30);
    }
}

public sealed class OrderValidator : FluentValidator<Order>
{
    protected override void DefineRules(IConstraintBuilder<Order> b)
    {
        b.Property(x => x.Id)
            .Positive();

        b.Property(x => x.Amount)
            .Positive();

        b.Property(x => x.Status)
            .NotEmpty();
    }
}

[ValidationContext("Direct")]
public sealed class UserDirectValidator(IUserRepository repo) : IDirectValidator<User>
{
    public async ValueTask<ValidationReport> ValidateAsync(
        User instance,
        IServiceProvider serviceProvider,
        IValidationCollector collector,
        CancellationToken ct = default)
    {
        if (instance.Email is not null)
        {
            var exists = await repo.EmailExistsAsync(instance.Email, ct);
            if (exists)
                collector.Collect(
                    ValidationStatus.Error(ValidationCode.NotUnique, nameof(User.Email)));
        }

        return collector.ToReport();
    }
}

public static class TestProviderFactory
{
    public static (IKeyedServiceProvider scope, IValidationProvider provider) Create(
        Action<IServiceBuilder>? configure = null)
    {
        var services = new ServiceBuilder();
        services.AddValidation();
        services.AddValidators(Assembly.GetExecutingAssembly(), true);
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        configure?.Invoke(services);

        var sp = services.Build();
        var scope = sp.CreateScope();
        var provider = scope.GetRequiredService<IValidationProvider>();
        return (scope, provider);
    }
}