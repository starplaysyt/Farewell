using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Logging;
using Farewell.Abstractions.StateRules;
using Farewell.DI;
using Farewell.StateRules;

namespace Farewell.Debug.Tests.StateRules;

file sealed class StateModel
{
    public bool IsValid { get; set; }
}

file sealed class FixingStateRule(StateModel model) : IStateRule
{
    public Task<bool> ValidateAsync(CancellationToken ct = default) =>
        Task.FromResult(model.IsValid);

    public Task TryFixAsync(CancellationToken ct = default)
    {
        model.IsValid = true;
        return Task.CompletedTask;
    }
}

file sealed class ReadOnlyStateRule(StateModel model) : IStateRule
{
    public Task<bool> ValidateAsync(CancellationToken ct = default) =>
        Task.FromResult(model.IsValid);

    public Task TryFixAsync(CancellationToken ct = default) => Task.CompletedTask;
}

file sealed class ThrowingFixRule(StateModel model) : IStateRule
{
    public Task<bool> ValidateAsync(CancellationToken ct = default) =>
        Task.FromResult(model.IsValid);

    public Task TryFixAsync(CancellationToken ct = default) =>
        throw new InvalidOperationException("Can't fix this rule, sorry.");
}

public class StateRulesEndToEndTests
{
    [Fact]
    public void UseStateRulesAsync_Succeeds_WhenStateAlreadyValid()
    {
        var builder = new ServiceBuilder();
        builder.AddSingleton((_) => new StateModel { IsValid = true });
        builder.AddStateRule<ReadOnlyStateRule>();

        using var scopeProvider = builder.Build();

        var result = scopeProvider.UseStateRules();

        Assert.Same(scopeProvider, result);
    }

    [Fact]
    public void UseStateRulesAsync_FixesState_WhenInvalidButFixable()
    {
        var builder = new ServiceBuilder();
        var model = new StateModel { IsValid = false };
        builder.AddSingleton((_) => model);
        builder.AddStateRule<FixingStateRule>();

        using var scopeProvider = builder.Build();

        scopeProvider.UseStateRules();

        Assert.True(model.IsValid);
    }

    [Fact]
    public void UseStateRulesAsync_Throws_WhenInvalidAndRuleCannotActuallyFix()
    {
        var builder = new ServiceBuilder();
        builder.AddSingleton((_) =>new StateModel { IsValid = false });
        builder.AddStateRule<ReadOnlyStateRule>();

        using var scopeProvider = builder.Build();

        Assert.Throws<StateRulesValidationException>(
            () => scopeProvider.UseStateRules());
    }

    [Fact]
    public void UseStateRulesAsync_Throws_WhenFixThrowsException()
    {
        var builder = new ServiceBuilder();
        builder.AddSingleton((_) => new StateModel { IsValid = false });
        builder.AddStateRule<ThrowingFixRule>();

        using var scopeProvider = builder.Build();

        Assert.Throws<StateRulesValidationException>(
            () => scopeProvider.UseStateRules());
    }

    [Fact]
    public void UseStateRulesAsync_Succeeds_WithLargeNumberOfValidRules()
    {
        var builder = new ServiceBuilder();
        builder.AddSingleton((_) => new StateModel { IsValid = true });

        for (var i = 0; i < 100; i++)
            builder.AddStateRule<ReadOnlyStateRule>();

        using var scopeProvider = builder.Build();

        var result = scopeProvider.UseStateRules();

        Assert.Same(scopeProvider, result);
    }

    [Fact]
    public void UseStateRulesAsync_Throws_WhenManyRulesShareInvalidState()
    {
        var builder = new ServiceBuilder();
        builder.AddSingleton((_) => new StateModel { IsValid = false });

        for (var i = 0; i < 10; i++)
            builder.AddStateRule<ReadOnlyStateRule>();

        using var scopeProvider = builder.Build();

        Assert.Throws<StateRulesValidationException>(
            () => scopeProvider.UseStateRules());
    }

    [Fact]
    public void UseStateRulesAsync_WritesLogs_OnFixAndOnFailure()
    {
        var fakeLogger = new FakeLogger();

        var builder = new ServiceBuilder();
        builder.AddSingleton((_) => new StateModel { IsValid = false });
        builder.AddStateRule<ReadOnlyStateRule>();

        using var scopeProvider = builder.Build();

        Assert.Throws<StateRulesValidationException>(
            () => scopeProvider.UseStateRules(fakeLogger));

        Assert.Contains(fakeLogger.Entries, e => e.Level == LogLevel.Warn);
        Assert.Contains(fakeLogger.Entries, e => e.Level == LogLevel.Error);
    }

    [Fact]
    public void UseStateRulesAsync_WritesInformationLog_WhenFixSucceeds()
    {
        var fakeLogger = new FakeLogger();

        var builder = new ServiceBuilder();
        builder.AddSingleton((_) => new StateModel { IsValid = false });
        builder.AddStateRule<FixingStateRule>();

        using var scopeProvider = builder.Build();

        scopeProvider.UseStateRules(logger: fakeLogger);

        Assert.Contains(fakeLogger.Entries, e => e.Level == LogLevel.Warn);
        Assert.Contains(fakeLogger.Entries, e => e.Level == LogLevel.Info);
    }
}