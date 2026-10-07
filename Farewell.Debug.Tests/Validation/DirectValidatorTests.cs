using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.Validation;

public sealed class DirectValidatorTests
{
    [Fact]
    public async Task DirectValidator_InjectsRepositoryFromDI()
    {
        var (scope, provider) = ProviderFactory.CreateProvider();
        using (scope)
        {
            var repo = scope.GetRequiredService<IRankRepository>();
            repo.Seed("AdminName", "ADM");

            var rank = new Rank { Name = "AdminName", ShortName = "ADM" };
            var report = await provider.ValidateAllAsync(rank, ct: TestContext.Current.CancellationToken);

            Assert.False(report.IsSuccess);
            Assert.Equal(2, report.Errors!.Count);
            Assert.All(report.Errors!, e => Assert.Equal(ValidationCode.NotUnique, e.Code));
        }
    }

    [Fact]
    public async Task DirectValidator_PassesWhenUnique()
    {
        var (scope, provider) = ProviderFactory.CreateProvider();
        using (scope)
        {
            var rank = new Rank { Name = "NewRank", ShortName = "NEW" };
            var report = await provider.ValidateAllAsync(rank, ct: TestContext.Current.CancellationToken);
            Assert.True(report.IsSuccess);
        }
    }

    [Fact]
    public async Task DirectValidator_IsScoped_NewInstancePerScope()
    {
        var root = ProviderFactory.BuildServices();

        IAsyncValidator<Rank> v1, v2;

        using (var scope = root.CreateScope())
            v1 = scope.GetRequiredKeyedService<IAsyncValidator<Rank>>("Default");

        using (var scope = root.CreateScope())
            v2 = scope.GetRequiredKeyedService<IAsyncValidator<Rank>>("Default");

        Assert.NotSame(v1, v2);
    }

    [Fact]
    public async Task DirectValidator_SharesRepositoryWithinScope()
    {
        var root = ProviderFactory.BuildServices();

        using var scope = root.CreateScope();
        var repo = scope.GetRequiredService<IRankRepository>();
        repo.Seed("Shared", "SHR");

        var provider = scope.GetRequiredService<IValidationProvider>();

        var rank = new Rank { Name = "Shared", ShortName = "SHR" };
        var report = await provider.ValidateAllAsync(rank, ct: TestContext.Current.CancellationToken);
        
        Assert.False(report.IsSuccess);
    }
}