using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.StateRules;
using Farewell.DI;
using Farewell.StateRules;

namespace Farewell.Debug.Tests.StateRules;

public class StateRulesServiceBuilderExtensionsTests
{
    private sealed class FakeStateRule : IStateRule
    {
        public Task<bool> ValidateAsync(CancellationToken ct = default) => Task.FromResult(true);
        public Task TryFixAsync(CancellationToken ct = default) => Task.CompletedTask;
    }

    [Fact]
    public void AddStateRule_ReturnsSameBuilder_ForFluentChaining()
    {
        var builder = new ServiceBuilder();

        var result = builder.AddStateRule<FakeStateRule>();

        Assert.Same(builder, result);
    }

    [Fact]
    public void AddStateRule_RegistersRule_ResolvableAsIStateRule()
    {
        var builder = new ServiceBuilder();
        builder.AddStateRule<FakeStateRule>();

        using var scopeProvider = builder.Build();
        using var scope = scopeProvider.CreateScope();

        var rules = scope.GetServices<IStateRule>().ToList();

        Assert.Single(rules);
        Assert.IsType<FakeStateRule>(rules[0]);
    }

    [Fact]
    public void AddStateRule_RegistersAsScoped_DifferentScopesGetDifferentInstances()
    {
        var builder = new ServiceBuilder();
        builder.AddStateRule<FakeStateRule>();

        using var scopeProvider = builder.Build();

        using var scope1 = scopeProvider.CreateScope();
        using var scope2 = scopeProvider.CreateScope();

        var rule1 = scope1.GetServices<IStateRule>().Single();
        var rule2 = scope2.GetServices<IStateRule>().Single();

        Assert.NotSame(rule1, rule2);
    }

    [Fact]
    public void AddStateRule_RegistersAsScoped_SameInstanceWithinSameScope()
    {
        var builder = new ServiceBuilder();
        builder.AddStateRule<FakeStateRule>();

        using var scopeProvider = builder.Build();
        using var scope = scopeProvider.CreateScope();

        var first = scope.GetServices<IStateRule>().Single();
        var second = scope.GetServices<IStateRule>().Single();

        Assert.Same(first, second);
    }

    [Fact]
    public void AddStateRule_AllowsMultipleDifferentRules_ToBeResolvedTogether()
    {
        var builder = new ServiceBuilder();
        builder.AddStateRule<FakeStateRule>();
        builder.AddStateRule<AnotherFakeStateRule>();

        using var scopeProvider = builder.Build();
        using var scope = scopeProvider.CreateScope();

        var rules = scope.GetServices<IStateRule>().ToList();

        Assert.Equal(2, rules.Count);
        Assert.Contains(rules, r => r is FakeStateRule);
        Assert.Contains(rules, r => r is AnotherFakeStateRule);
    }

    private sealed class AnotherFakeStateRule : IStateRule
    {
        public Task<bool> ValidateAsync(CancellationToken ct = default) => Task.FromResult(true);
        public Task TryFixAsync(CancellationToken ct = default) => Task.CompletedTask;
    }
}