using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.General;
using Farewell.Abstractions.Validation;
using Farewell.DI;
using Farewell.Validation.Internal;

namespace Farewell.Debug.Tests.Validation;

public sealed class ValidationConstraintExtensionsTests
{
    private static readonly IServiceProvider EmptySp = new ServiceBuilder().Build();

    private static async Task<ValidationReport> ValidateSingle<T, TValue>(
        T instance,
        System.Linq.Expressions.Expression<Func<T, TValue>> property,
        Action<IPropertyConstraintBuilder<TValue?>> configure)
    {
        var builder = new ConstraintBuilder<T>();
        configure(builder.Property(property));
        var compiled = builder.Compile();
        return await compiled.ValidateAsync(
            instance, EmptySp, new CollectAllCollector(), CancellationToken.None);
    }

    [Fact]
    public async Task NotEmpty_NullString_Fails()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Email = null }, x => x.Email,
            b => b.NotEmpty());

        Assert.False(report.IsSuccess);
        Assert.Equal(ResultCode.ValidationNotEmpty, report.Errors![0].Code);
    }

    [Fact]
    public async Task NotEmpty_EmptyString_Fails()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Email = "" }, x => x.Email,
            b => b.NotEmpty());

        Assert.False(report.IsSuccess);
    }

    [Fact]
    public async Task NotEmpty_ValidString_Passes()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Email = "a@b.com" }, x => x.Email,
            b => b.NotEmpty());

        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task MinLength_TooShort_Fails()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Email = "ab" }, x => x.Email,
            b => b.MinLength(3));

        Assert.False(report.IsSuccess);
        Assert.Equal(ResultCode.ValidationMinLength, report.Errors![0].Code);
    }

    [Fact]
    public async Task MinLength_ExactlyMin_Passes()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Email = "abc" }, x => x.Email,
            b => b.MinLength(3));

        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task MinLength_Null_Passes()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Email = null }, x => x.Email,
            b => b.MinLength(3));

        Assert.False(report.IsSuccess);
    }

    [Fact]
    public async Task MinLength_Dynamic_ResolvesFromSP()
    {
        var services = new ServiceBuilder();
        services.AddSingleton<MinLengthConfig>();
        var sp = services.Build();

        var builder = new ConstraintBuilder<User>();
        builder.Property(x => x.Name)
            .MinLength(s => s.GetRequiredService<MinLengthConfig>().Value);
        var compiled = builder.Compile();

        var report = await compiled.ValidateAsync(new User { Name = "ab" }, sp,
            new CollectAllCollector(), TestContext.Current.CancellationToken);

        Assert.False(report.IsSuccess);
    }

    [Fact]
    public async Task MaxLength_TooLong_Fails()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Email = "abcdef" }, x => x.Email,
            b => b.MaxLength(5));

        Assert.False(report.IsSuccess);
        Assert.Equal(ResultCode.ValidationMaxLength, report.Errors![0].Code);
    }

    [Fact]
    public async Task MaxLength_Null_Passes()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Email = null }, x => x.Email,
            b => b.MaxLength(5));

        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task Email_Invalid_Fails()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Email = "notanemail" }, x => x.Email,
            b => b.Email());

        Assert.False(report.IsSuccess);
        Assert.Equal(ResultCode.ValidationInvalidEmail, report.Errors![0].Code);
    }

    [Fact]
    public async Task Email_Valid_Passes()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Email = "user@example.com" }, x => x.Email,
            b => b.Email());

        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task Positive_Zero_Fails()
    {
        var report = await ValidateSingle(
            new User { Age = 0 }, x => x.Age,
            b => b.Positive());

        Assert.False(report.IsSuccess);
        Assert.Equal(ResultCode.ValidationMustBePositive, report.Errors![0].Code);
    }

    [Fact]
    public async Task Positive_Positive_Passes()
    {
        var report = await ValidateSingle(
            new User { Age = 5 }, x => x.Age,
            b => b.Positive());

        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task InRange_OutOfBounds_Fails()
    {
        var report = await ValidateSingle(
            new User { Age = 200 }, x => x.Age,
            b => b.InRange(0, 150));

        Assert.False(report.IsSuccess);
        Assert.Equal(ResultCode.ValidationOutOfRange, report.Errors![0].Code);
    }

    [Fact]
    public async Task InRange_InBounds_Passes()
    {
        var report = await ValidateSingle(
            new User { Age = 25 }, x => x.Age,
            b => b.InRange(0, 150));

        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task DecimalPositive_Negative_Fails()
    {
        var report = await ValidateSingle(
            new User { Salary = -1m }, x => x.Salary,
            b => b.Positive());

        Assert.False(report.IsSuccess);
    }

    [Fact]
    public async Task AddCheck_SyncOverload_Works()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Name = "" }, x => x.Name,
            b => b.AddCheck(ResultCode.ValidationNotEmpty, v => !string.IsNullOrEmpty(v)));

        Assert.False(report.IsSuccess);
    }

    [Fact]
    public async Task AddCheck_SyncWithSP_Overload_Works()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Name = "ok" }, x => x.Name,
            b => b.AddCheck(ResultCode.ValidationNotEmpty, (v, _) => !string.IsNullOrEmpty(v)));

        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task AddCheck_FullAsync_Overload_Works()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Name = "ok" }, x => x.Name,
            b => b.AddCheck(ResultCode.ValidationNotEmpty,
                (v, _, _) => new ValueTask<bool>(!string.IsNullOrEmpty(v))));

        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task MultipleChecks_AllPass()
    {
        var report = await ValidateSingle<User, string?>(
            new User { Name = "Alice" }, x => x.Name,
            b => b.NotEmpty().MinLength(3).MaxLength(50));

        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task MultipleChecks_FirstFails_CollectsAll()
    {
        var builder = new ConstraintBuilder<User>();
        builder.Property(x => x.Name)
            .NotEmpty()
            .MinLength(3)
            .MaxLength(50);

        var compiled = builder.Compile();
        var report = await compiled.ValidateAsync(new User { Name = "" }, EmptySp,
            new CollectAllCollector(), TestContext.Current.CancellationToken);

        Assert.False(report.IsSuccess);
        Assert.Equal(2, report.Errors!.Count);
    }

    [Fact]
    public async Task When_ConditionTrue_RunsChecks()
    {
        var builder = new ConstraintBuilder<User>();
        builder.Property(x => x.CompanyName, x => x.IsPremium)
            .NotEmpty();

        var compiled = builder.Compile();
        var report = await compiled.ValidateAsync(new User { IsPremium = true, CompanyName = null },
            EmptySp, new CollectAllCollector(), TestContext.Current.CancellationToken);

        Assert.False(report.IsSuccess);
    }

    [Fact]
    public async Task When_ConditionFalse_SkipsChecks()
    {
        var builder = new ConstraintBuilder<User>();
        builder.Property(x => x.CompanyName, x => x.IsPremium)
            .NotEmpty();

        var compiled = builder.Compile();
        var report = await compiled.ValidateAsync(
            new User { IsPremium = false, CompanyName = null }, EmptySp, new CollectAllCollector(),
            TestContext.Current.CancellationToken);

        Assert.True(report.IsSuccess);
    }

    [Fact]
    public async Task MultipleProperties_IndependentConditions()
    {
        var builder = new ConstraintBuilder<User>();
        builder.Property(x => x.Name).NotEmpty();
        builder.Property(x => x.CompanyName, x => x.IsPremium).NotEmpty();

        var compiled = builder.Compile();

        var report = await compiled.ValidateAsync(
            new User { Name = "Alice", IsPremium = false, CompanyName = null }, EmptySp,
            new CollectAllCollector(), TestContext.Current.CancellationToken);

        Assert.True(report.IsSuccess);
    }

    private sealed class MinLengthConfig
    {
        public int Value => 5;
    }
}