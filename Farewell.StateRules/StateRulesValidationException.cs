using Farewell.Abstractions.Extensions;

namespace Farewell.StateRules;

public sealed class StateRulesValidationException(IReadOnlyList<StateRuleFailure> failures)
    : AggregateException(
        $"{failures.Count} state rule(s) failed: {string.Join(", ", failures.Select(f => f.RuleName))}",
        failures.Select(f => f.Exception))
{
    public IReadOnlyList<StateRuleFailure> Failures { get; } = failures;
}