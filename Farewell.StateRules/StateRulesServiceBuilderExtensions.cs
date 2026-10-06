using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.StateRules;

namespace Farewell.StateRules;

public sealed record StateRuleFailure(string RuleName, Exception Exception);

public static class StateRulesServiceBuilderExtensions
{
    public static IServiceBuilder AddStateRule<TStateRule>(this IServiceBuilder serviceBuilder)
        where TStateRule : class, IStateRule
    {
        serviceBuilder.AddScoped<IStateRule, TStateRule>();
        return serviceBuilder;
    }
}