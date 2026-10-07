using Farewell.Abstractions.DI;
using Farewell.Abstractions.StateRules;

namespace Farewell.Abstractions.Extensions;

public sealed record StateRuleFailure(string RuleName, Exception Exception);

public static class StateRulesServiceBuilderExtensions
{
    public static IServiceBuilder AddStateRule<TStateRule>(this IServiceBuilder serviceBuilder)
        where TStateRule : class, IStateRule
    {
        serviceBuilder.AddScoped<IStateRule, TStateRule>();
        return serviceBuilder;
    }

    public static IServiceBuilder AddStateRule<TStateRule>(this IServiceBuilder serviceBuilder,
        Func<IServiceProvider, TStateRule> factory)
        where TStateRule : class, IStateRule
    {
        serviceBuilder.AddScoped<IStateRule>(factory);
        return serviceBuilder;
    }
}