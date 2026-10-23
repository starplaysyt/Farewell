using Farewell.Abstractions.DI;
using Farewell.Abstractions.General;
using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class FluentValidatorTests : IDisposable
{
    private readonly IKeyedServiceProvider _scope;
    private readonly IValidationProvider _provider;

    public FluentValidatorTests()
    {
        (_scope, _provider) = TestProviderFactory.Create();
    }

    public void Dispose() => _scope.Dispose();

    [Fact]
    public async Task ValidUser_DefaultContext_Passes()
    {
        var user = new User
        {
            Name = "Alice",
            Email = "alice@example.com",
            Age = 30
        };

        var report = await _provider.ValidateAsync(user, ct: TestContext.Current.CancellationToken);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task EmptyName_DefaultContext_Fails()
    {
        var user = new User
        {
            Name = "",
            Email = "alice@example.com"
        };

        var report = await _provider.ValidateAsync(user, ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e.Code == ResultCode.ValidationNotEmpty && e.PropertyName == nameof(User.Name));
    }

    [Fact]
    public async Task ShortName_DefaultContext_FailsMinLength()
    {
        var user = new User
        {
            Name = "Al",
            Email = "alice@example.com"
        };

        var report = await _provider.ValidateAsync(user, ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e.Code == ResultCode.ValidationMinLength && e.PropertyName == nameof(User.Name));
    }

    [Fact]
    public async Task InvalidEmail_Fails()
    {
        var user = new User
        {
            Name = "Alice",
            Email = "notanemail"
        };

        var report = await _provider.ValidateAsync(user, ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e.Code == ResultCode.ValidationInvalidEmail);
    }

    [Fact]
    public async Task PremiumUser_NoCompany_Fails()
    {
        var user = new User
        {
            Name = "Alice",
            Email = "alice@example.com",
            IsPremium = true,
            CompanyName = null
        };

        var report = await _provider.ValidateAsync(user, ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e.PropertyName == nameof(User.CompanyName));
    }

    [Fact]
    public async Task NonPremiumUser_NoCompany_Passes()
    {
        var user = new User
        {
            Name = "Alice",
            Email = "alice@example.com",
            IsPremium = false,
            CompanyName = null
        };

        var report = await _provider.ValidateAsync(user, ct: TestContext.Current.CancellationToken);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task StrictContext_EnforcesStricterRules()
    {
        var user = new User { Name = "Bob" }; // 3 chars, OK for Default, fail for Strict (min 5)

        var report = await _provider.ValidateAsync(user, context: "Strict",
            ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e.Code == ResultCode.ValidationMinLength && e.PropertyName == nameof(User.Name));
    }

    [Fact]
    public async Task BreakOnFirst_ReturnsSingleError()
    {
        var user = new User
        {
            Name = "",
            Email = ""
        };

        var report = await _provider.ValidateAsync(user, breakOnFirst: true,
            ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Single(report.Errors!);
    }

    [Fact]
    public async Task CollectAll_ReturnsMultipleErrors()
    {
        var user = new User
        {
            Name = "",
            Email = ""
        };

        var report = await _provider.ValidateAsync(user, breakOnFirst: false,
            ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.True(report.Errors!.Count > 1);
    }

    [Fact]
    public void SyncWrapper_Works()
    {
        var user = new User
        {
            Name = "Alice",
            Email = "alice@example.com"
        };

        var report = _provider.Validate(user);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task OrderValidator_ValidOrder_Passes()
    {
        var order = new Order { Id = 1, Amount = 99.99m, Status = "New" };

        var report =
            await _provider.ValidateAsync(order, ct: TestContext.Current.CancellationToken);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task OrderValidator_ZeroId_Fails()
    {
        var order = new Order { Id = 0, Amount = 10m, Status = "New" };

        var report =
            await _provider.ValidateAsync(order, ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e.Code == ResultCode.ValidationMustBePositive && e.PropertyName == nameof(Order.Id));
    }
}