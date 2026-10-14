using Farewell.Abstractions.Extensions;
using Farewell.DI;
using Farewell.Validation.Constraints;

namespace Farewell.Debug.Tests.Validation;

public sealed class ConstraintsTests
{
    private readonly IServiceProvider _sp = new ServiceBuilder().Build();

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("x", true)]
    public async Task NotEmpty_Check(string? value, bool expected)
    {
        var c = new StringConstraints.NotEmptyConstraint();
        var result = await c.CheckAsync(value, _sp, TestContext.Current.CancellationToken);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("a", 2, false)]
    [InlineData("ab", 2, true)]
    [InlineData("abc", 2, true)]
    [InlineData(null, 2, false)]
    public async Task MinLength_Static_Check(string? value, int min, bool expected)
    {
        var c = new StringConstraints.MinLengthConstraint(min);
        Assert.Equal(expected,
            await c.CheckAsync(value, _sp, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task MinLength_Dynamic_ReadsFromSp()
    {
        var services = new ServiceBuilder();
        services.AddSingleton<IAppConfig>((_) => new TestAppConfig { MinNameLength = 3 });
        var sp = services.Build();

        var c = new StringConstraints.MinLengthConstraint(s =>
            s.GetRequiredService<IAppConfig>().MinNameLength);

        Assert.False(await c.CheckAsync("ab", sp, TestContext.Current.CancellationToken));
        Assert.True(await c.CheckAsync("abc", sp, TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData("ab", 5, true)]
    [InlineData("abcde", 5, true)]
    [InlineData("abcdef", 5, false)]
    [InlineData(null, 5, true)]
    public async Task MaxLength_Static_Check(string? value, int max, bool expected)
    {
        var c = new StringConstraints.MaxLengthConstraint(max);
        Assert.Equal(expected,
            await c.CheckAsync(value, _sp, TestContext.Current.CancellationToken));
    }

    [Theory]
    [InlineData("test@mail.com", @"^[^@\s]+@[^@\s]+\.[^@\s]+$", true)]
    [InlineData("bad", @"^[^@\s]+@[^@\s]+\.[^@\s]+$", false)]
    [InlineData(null, @"^.+$", false)]
    public async Task Matches_Check(string? value, string pattern, bool expected)
    {
        var c = new StringConstraints.MatchesConstraint(pattern);
        Assert.Equal(expected, await c.CheckAsync(value, _sp, CancellationToken.None));
    }

    [Theory]
    [InlineData(0, 0, 10, true)]
    [InlineData(10, 0, 10, true)]
    [InlineData(5, 0, 10, true)]
    [InlineData(-1, 0, 10, false)]
    [InlineData(11, 0, 10, false)]
    public async Task OutOfRange_Int_Static_Check(int value, int min, int max, bool expected)
    {
        var c = new NumericConstraints.OutOfRangeConstraint<int>(min, max);
        Assert.Equal(expected,
            await c.CheckAsync(value, _sp, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task OutOfRange_Dynamic_ReadsFromSp()
    {
        var services = new ServiceBuilder();
        services.AddSingleton<IAppConfig>((_) => new TestAppConfig
        {
            MinNameLength = 10,
            MaxNameLength = 20
        });
        var sp = services.Build();

        var c = new NumericConstraints.OutOfRangeConstraint<int>(
            s => s.GetRequiredService<IAppConfig>().MinNameLength,
            s => s.GetRequiredService<IAppConfig>().MaxNameLength);

        Assert.False(await c.CheckAsync(5, sp, TestContext.Current.CancellationToken));
        Assert.True(await c.CheckAsync(15, sp, TestContext.Current.CancellationToken));
        Assert.False(await c.CheckAsync(25, sp, TestContext.Current.CancellationToken));
    }
}