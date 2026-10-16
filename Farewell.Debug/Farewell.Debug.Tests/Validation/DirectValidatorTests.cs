using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class DirectValidatorTests : IDisposable
{
    private readonly IKeyedServiceProvider _scope;
    private readonly IValidationProvider _provider;
    private readonly InMemoryUserRepository _repo;

    public DirectValidatorTests()
    {
        _repo = new InMemoryUserRepository();
        (_scope, _provider) = TestProviderFactory.Create(services =>
        {
            services.AddSingleton<IUserRepository>(_ => _repo);
        });
    }

    public void Dispose() => _scope.Dispose();

    [Fact]
    public async Task UniqueEmail_Passes()
    {
        var user = new User { Name = "Alice", Email = "alice@example.com" };
        var report = await _provider.ValidateAsync(user, context: "Direct",
            ct: TestContext.Current.CancellationToken);
        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task DuplicateEmail_FailsNotUnique()
    {
        _repo.Add("taken@example.com");
        var user = new User { Name = "Bob", Email = "taken@example.com" };
        var report = await _provider.ValidateAsync(user, context: "Direct",
            ct: TestContext.Current.CancellationToken);
        Assert.False(report.IsSuccess);
        Assert.Contains(report.Errors!, e =>
            e.Code == ValidationCode.NotUnique && e.PropertyName == nameof(User.Email));
    }
}