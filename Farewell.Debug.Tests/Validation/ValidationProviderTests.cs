using Farewell.Abstractions.Validation;
using Farewell.Application.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationProviderTests
{
    private readonly IValidationProvider _provider = ProviderFactory.Create();

    [Fact]
    public void ValidateAll_ValidUser_ReturnsOk()
    {
        var user = new User { Name = "John", Email = "john@mail.com", Age = 25 };
        var report = _provider.ValidateAll(user);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public void ValidateAll_InvalidUser_ReturnsErrors()
    {
        var user = new User { Name = "", Email = "bad", Age = 200 };
        var report = _provider.ValidateAll(user);
        Assert.False(report.IsSuccess);
    }

    [Fact]
    public void ValidateBreak_ValidUser_ReturnsOk()
    {
        var user = new User { Name = "John", Email = "john@mail.com", Age = 25 };
        var status = _provider.ValidateBreak(user);
        Assert.True(status.IsSuccess);
    }

    [Fact]
    public void ValidateBreak_InvalidUser_ReturnsError()
    {
        var user = new User { Name = "", Email = "bad", Age = 200 };
        var status = _provider.ValidateBreak(user);
        Assert.False(status.IsSuccess);
    }

    [Fact]
    public void ValidateAll_CreateContext_UsesCreateValidator()
    {
        // UserCreateValidator requires MinLength(5)
        // Name = "Jo" - passes base (MinLength 2), not passes Create (MinLength 5)
        var user = new User { Name = "Jo", Email = "john@mail.com", Age = 25 };

        var defaultReport = _provider.ValidateAll(user);
        var createReport = _provider.ValidateAll(user, "Create");

        Assert.True(defaultReport.IsSuccess);
        Assert.False(createReport.IsSuccess);
    }

    [Fact]
    public void ValidateBreak_CreateContext_UsesCreateValidator()
    {
        var user = new User { Name = "Jo" };
        var status = _provider.ValidateBreak(user, "Create");
        Assert.False(status.IsSuccess);
    }

    [Fact]
    public void ValidateAll_NoValidator_ReturnsOk()
    {
        var entity = new UserWithoutValidator { Name = "" };
        var report = _provider.ValidateAll(entity);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public void ValidateBreak_NoValidator_ReturnsOk()
    {
        var entity = new UserWithoutValidator { Name = "" };
        var status = _provider.ValidateBreak(entity);
        Assert.True(status.IsSuccess);
    }

    [Fact]
    public void ValidateAll_UnknownContext_ReturnsOk()
    {
        var user = new User { Name = "John", Email = "john@mail.com", Age = 25 };
        var report = _provider.ValidateAll(user, "UnknownContext");
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public void Resolve_DuplicateContext_ThrowsInvalidOperationException()
    {
        var entity = new UserDuplicate();
        Assert.Throws<InvalidOperationException>(() => _provider.ValidateAll(entity));
    }

    [Fact]
    public void ValidateAll_CalledMultipleTimes_ReturnsSameResult()
    {
        var user = new User { Name = "John", Email = "john@mail.com", Age = 25 };

        var report1 = _provider.ValidateAll(user);
        var report2 = _provider.ValidateAll(user);
        var report3 = _provider.ValidateAll(user);

        Assert.Equal(report1.IsSuccess, report2.IsSuccess);
        Assert.Equal(report2.IsSuccess, report3.IsSuccess);
    }

    [Fact]
    public void InvalidateCache_AfterInvalidation_StillWorks()
    {
        var provider = (ValidationProvider)_provider;
        var user = new User { Name = "John", Email = "john@mail.com", Age = 25 };

        var before = provider.ValidateAll(user);
        provider.InvalidateCache();
        var after = provider.ValidateAll(user);

        Assert.Equal(before.IsSuccess, after.IsSuccess);
    }

    [Fact]
    public void DirectValidator_ValidateAll_Works()
    {
        var validator = new DirectUserValidator();
        var user = new User { Name = "" };
        var report = validator.ValidateAll(user);

        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e is { Code: ValidationCode.NotEmpty, PropertyName: nameof(User.Name) });
    }

    [Fact]
    public void DirectValidator_ValidateBreak_Works()
    {
        var validator = new DirectUserValidator();
        var user = new User { Name = "" };
        var status = validator.ValidateBreak(user);

        Assert.False(status.IsSuccess);
        Assert.Equal(ValidationCode.NotEmpty, status.Code);
    }

    [Fact]
    public void DirectValidator_ValidUser_ReturnsOk()
    {
        var validator = new DirectUserValidator();
        var user = new User { Name = "John" };

        var report = validator.ValidateAll(user);
        var status = validator.ValidateBreak(user);

        Assert.True(report.IsSuccess);
        Assert.True(status.IsSuccess);
    }
}