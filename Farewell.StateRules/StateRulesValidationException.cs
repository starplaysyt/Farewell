using Farewell.Abstractions.StateRules;

namespace Farewell.StateRules;

public sealed class StateRulesValidationException : AggregateException
{
    public IReadOnlyList<StateRuleFailure> Failures { get; }

    public StateRulesValidationException(IReadOnlyList<StateRuleFailure> failures)
        : base(
            $"{failures.Count} state rule(s) failed: {string.Join(", ", failures.Select(f => f.RuleName))}",
            failures.Select(f => f.Exception))
    {
        Failures = failures;
    }
}