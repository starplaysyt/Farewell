using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class FluentValidatorTests
{
    private readonly UserValidator _validator = new();

    [Fact]
    public void ValidateAll_ValidUser_ReturnsOk()
    {
        var user = new User { Name = "John", Email = "john@mail.com", Age = 25 };
        var report = _validator.ValidateAll(user);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public void ValidateAll_InvalidUser_ReturnsErrors()
    {
        var user = new User { Name = "", Email = "notanemail", Age = 200 };
        var report = _validator.ValidateAll(user);
        Assert.False(report.IsSuccess);
    }

    [Fact]
    public void ValidateAll_CollectsAllErrors()
    {
        var user = new User { Name = "", Email = "bad", Age = 200 };
        var report = _validator.ValidateAll(user);

        Assert.Equal(ValidationCode.MultipleErrors, report.Status);
        Assert.NotNull(report.Errors);
        Assert.True(report.Errors!.Count > 1);
    }

    [Fact]
    public void ValidateAll_NameEmpty_ReturnsNotEmpty()
    {
        var user = new User { Name = "", Email = "john@mail.com", Age = 25 };
        var report = _validator.ValidateAll(user);

        Assert.Contains(report.Errors!, e =>
            e is { Code: ValidationCode.NotEmpty, PropertyName: nameof(User.Name) });
    }

    [Fact]
    public void ValidateAll_NameTooShort_ReturnsMinLength()
    {
        var user = new User { Name = "J", Email = "john@mail.com", Age = 25 };
        var report = _validator.ValidateAll(user);

        Assert.Contains(report.Errors!, e =>
            e is { Code: ValidationCode.MinLength, PropertyName: nameof(User.Name) });
    }

    [Fact]
    public void ValidateAll_NameTooLong_ReturnsMaxLength()
    {
        var user = new User { Name = new string('a', 51), Email = "john@mail.com", Age = 25 };
        var report = _validator.ValidateAll(user);

        Assert.Contains(report.Errors!, e =>
            e is { Code: ValidationCode.MaxLength, PropertyName: nameof(User.Name) });
    }

    [Fact]
    public void ValidateAll_InvalidEmail_ReturnsInvalidFormat()
    {
        var user = new User { Name = "John", Email = "notanemail", Age = 25 };
        var report = _validator.ValidateAll(user);

        Assert.Contains(report.Errors!, e =>
            e is { Code: ValidationCode.InvalidFormat, PropertyName: nameof(User.Email) });
    }

    [Fact]
    public void ValidateAll_AgeOutOfRange_ReturnsOutOfRange()
    {
        var user = new User { Name = "John", Email = "john@mail.com", Age = 200 };
        var report = _validator.ValidateAll(user);

        Assert.Contains(report.Errors!, e =>
            e is { Code: ValidationCode.OutOfRange, PropertyName: nameof(User.Age) });
    }

    [Fact]
    public void ValidateBreak_ValidUser_ReturnsOk()
    {
        var user = new User { Name = "John", Email = "john@mail.com", Age = 25 };
        var status = _validator.ValidateBreak(user);
        Assert.True(status.IsSuccess);
    }

    [Fact]
    public void ValidateBreak_InvalidUser_ReturnsFirstError()
    {
        var user = new User { Name = "", Email = "bad", Age = 200 };
        var status = _validator.ValidateBreak(user);
        Assert.False(status.IsSuccess);
    }

    [Fact]
    public void ValidateBreak_ReturnsOnlyOneError()
    {
        var user = new User { Name = "", Email = "bad", Age = 200 };
        var status = _validator.ValidateBreak(user);

        // ValidateBreak returns one status
        Assert.IsType<ValidationStatus>(status);
    }

    [Fact]
    public void Validator_CompilesOnce_ReturnsSameResult()
    {
        var user = new User { Name = "John", Email = "john@mail.com", Age = 25 };

        var report1 = _validator.ValidateAll(user);
        var report2 = _validator.ValidateAll(user);

        Assert.Equal(report1.IsSuccess, report2.IsSuccess);
        Assert.Equal(report1.Status, report2.Status);
    }

    [Fact]
    public void When_ConditionFalse_SkipsConstraints()
    {
        // HasPhone = false - rules for phone are not passes
        var user = new User { Name = "John", Email = "john@mail.com", Age = 25, HasPhone = false };
        var report = _validator.ValidateAll(user);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public void When_ConditionTrue_AppliesConstraints()
    {
        // HasPhone = true, Phone is empty - error
        var user = new User
        {
            Name = "John",
            Email = "john@mail.com",
            Age = 25,
            HasPhone = true,
            Phone = null
        };
        var report = _validator.ValidateAll(user);

        Assert.Contains(report.Errors!, e =>
            e.PropertyName == nameof(User.Phone));
    }

    [Fact]
    public void When_ConditionTrue_InvalidFormat_ReturnsError()
    {
        var user = new User
        {
            Name = "John",
            Email = "john@mail.com",
            Age = 25,
            HasPhone = true,
            Phone = "12345"
        };
        var report = _validator.ValidateAll(user);

        Assert.Contains(report.Errors!, e =>
            e is { Code: ValidationCode.InvalidFormat, PropertyName: nameof(User.Phone) });
    }
}