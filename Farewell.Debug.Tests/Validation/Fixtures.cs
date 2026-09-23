using Farewell.Abstractions.Attributes.Validation;
using Farewell.Abstractions.Validation;
using Farewell.Validation;
using Farewell.Validation.Extensions;

namespace Farewell.Debug.Tests.Validation;

[ValidatedBy(typeof(UserValidator))]
[ValidatedBy(typeof(UserCreateValidator), "Create")]
public class User
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int Age { get; set; }
    public string? Phone { get; set; }
    public bool HasPhone { get; set; }
}

public class UserWithoutValidator
{
    public string? Name { get; set; }
}

[ValidatedBy(typeof(UserValidator), "Default")]
[ValidatedBy(typeof(UserValidator), "Default")]
public class UserDuplicate { }

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

public sealed class UserCreateValidator : FluentValidator<User>
{
    protected override void DefineRules(PropertiesValidatorBuilder<User> builder)
    {
        builder.Rule(u => u.Name)
            .NotEmpty()
            .MinLength(5);
    }
}

public sealed class DirectUserValidator : IDirectValidator<User>
{
    public ValidationReport ValidateAll(User instance)
    {
        var errors = new List<ValidationStatus>();

        if (string.IsNullOrWhiteSpace(instance.Name))
            errors.Add(new ValidationStatus(ValidationCode.NotEmpty, nameof(User.Name)));

        return errors.Count > 0
            ? new ValidationReport(errors.ToArray())
            : ValidationReport.Ok;
    }

    public ValidationStatus ValidateBreak(User instance)
    {
        if (string.IsNullOrWhiteSpace(instance.Name))
            return new ValidationStatus(ValidationCode.NotEmpty, nameof(User.Name));

        return ValidationStatus.Ok;
    }
}

public static class ProviderFactory
{
    public static IValidationProvider Create()
        => new ValidationProvider(new AttributeValidatorResolver());
}

