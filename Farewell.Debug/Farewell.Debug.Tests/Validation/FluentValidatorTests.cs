using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class FluentValidatorTests
{
    [Fact]
    public async Task ValidateAllAsync_ValidUser_ReturnsOk()
    {
        var (scope, provider) = ProviderFactory.CreateProvider();
        using (scope)
        {
            var user = new User { Name = "John", Email = "john@mail.com", Age = 25 };
            var report = await provider.ValidateAllAsync(user, ct: TestContext.Current.CancellationToken);
            Assert.True(report.IsSuccess);
        }
    }

    [Fact]
    public async Task ValidateAllAsync_InvalidUser_CollectsAllErrors()
    {
        var (scope, provider) = ProviderFactory.CreateProvider();
        using (scope)
        {
            var user = new User { Name = "", Email = "bad", Age = 200 };
            var report = await provider.ValidateAllAsync(user, ct: TestContext.Current.CancellationToken);

            Assert.False(report.IsSuccess);
            Assert.Equal(ValidationCode.MultipleErrors, report.Status);
            Assert.True(report.Errors!.Count >= 3);
        }
    }

    [Fact]
    public async Task ValidateFirstAsync_InvalidUser_ReturnsFirstErrorOnly()
    {
        var (scope, provider) = ProviderFactory.CreateProvider();
        using (scope)
        {
            var user = new User { Name = "", Email = "bad", Age = 200 };
            var report = await provider.ValidateFirstAsync(user, ct: TestContext.Current.CancellationToken);

            Assert.False(report.IsSuccess);
            Assert.Single(report.Errors!);
        }
    }

    [Fact]
    public async Task Context_Default_UsesDefaultValidator()
    {
        var (scope, provider) = ProviderFactory.CreateProvider();
        using (scope)
        {
            var user = new User { Name = "Jo", Email = "j@m.co", Age = 25 };
            var report = await provider.ValidateAllAsync(user, ct: TestContext.Current.CancellationToken);
            Assert.True(report.IsSuccess);
        }
    }

    [Fact]
    public async Task Context_Create_UsesCreateValidator()
    {
        var (scope, provider) = ProviderFactory.CreateProvider();
        using (scope)
        {
            var user = new User { Name = "Jo" };
            var report = await provider.ValidateAllAsync(user, "Create", TestContext.Current.CancellationToken);
            Assert.False(report.IsSuccess);
            Assert.Equal(ValidationCode.MinLength, report.Status);
        }
    }

    [Fact]
    public async Task When_ConditionFalse_SkipsRules()
    {
        var (scope, provider) = ProviderFactory.CreateProvider();
        using (scope)
        {
            var user = new User
            {
                Name = "John", Email = "j@m.co", Age = 25,
                HasPhone = false, Phone = null
            };
            var report = await provider.ValidateAllAsync(user, ct: TestContext.Current.CancellationToken);
            Assert.True(report.IsSuccess);
        }
    }

    [Fact]
    public async Task When_ConditionTrue_AppliesRules()
    {
        var (scope, provider) = ProviderFactory.CreateProvider();
        using (scope)
        {
            var user = new User
            {
                Name = "John", Email = "j@m.co", Age = 25,
                HasPhone = true, Phone = null
            };
            var report = await provider.ValidateAllAsync(user, ct: TestContext.Current.CancellationToken);
            Assert.False(report.IsSuccess);
            Assert.Contains(report.Errors!, e => e.PropertyName == nameof(User.Phone));
        }
    }

    [Fact]
    public async Task When_ConditionTrue_InvalidFormat()
    {
        var (scope, provider) = ProviderFactory.CreateProvider();
        using (scope)
        {
            var user = new User
            {
                Name = "John", Email = "j@m.co", Age = 25,
                HasPhone = true, Phone = "12345"
            };
            var report = await provider.ValidateAllAsync(user, ct: TestContext.Current.CancellationToken);
            Assert.Contains(report.Errors!, e =>
                e.PropertyName == nameof(User.Phone) &&
                e.Code == ValidationCode.InvalidFormat);
        }
    }

    [Fact]
    public async Task DynamicConstraint_ReadsConfigFromSp()
    {
        var (scope, provider) = ProviderFactory.CreateProvider(s =>
        {
            s.AddSingleton<IAppConfig>((_) => new TestAppConfig
            {
                MinNameLength = 10,
                MaxNameLength = 100
            });
        });

        using (scope)
        {
            var user = new User { Name = "Short" };
            var report = await provider.ValidateAllAsync(user, "Dynamic", TestContext.Current.CancellationToken);

            Assert.False(report.IsSuccess);
            Assert.Equal(ValidationCode.MinLength, report.Status);
        }
    }

    [Fact]
    public async Task DynamicConstraint_PassesWhenConfigAllows()
    {
        var (scope, provider) = ProviderFactory.CreateProvider(s =>
        {
            s.AddSingleton<IAppConfig>((_) => new TestAppConfig
            {
                MinNameLength = 2,
                MaxNameLength = 100
            });
        });

        using (scope)
        {
            var user = new User { Name = "Good" };
            var report = await provider.ValidateAllAsync(user, "Dynamic", TestContext.Current.CancellationToken);
            Assert.True(report.IsSuccess);
        }
    }

    [Fact]
    public async Task FluentValidator_IsSingleton_CompilesOnce()
    {
        var root = ProviderFactory.BuildServices();
        
        IFluentValidator<User> v1, v2;

        using (var scope = root.CreateScope())
            v1 = scope.GetRequiredKeyedService<IAsyncValidator<User>>("Default")
                as IFluentValidator<User> ?? throw new InvalidOperationException();

        using (var scope = root.CreateScope())
            v2 = scope.GetRequiredKeyedService<IAsyncValidator<User>>("Default")
                as IFluentValidator<User> ?? throw new InvalidOperationException();

        Assert.Same(v1, v2);
    }
}