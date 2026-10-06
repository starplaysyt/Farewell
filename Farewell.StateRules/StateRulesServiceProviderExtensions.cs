using Farewell.Abstractions.DI;
using Farewell.Abstractions.Exceptions;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Logging;
using Farewell.Abstractions.StateRules;

namespace Farewell.StateRules;

public static class StateRulesServiceProviderExtensions
{
    public static IServiceProvider UseStateRules(
        this IServiceProvider serviceProvider,
        ILogger? logger = null)
    {
        if (serviceProvider is not IScopeProvider scopeProvider)
            throw new UnsupportedOperationException(
                "Given service provider doesn't support scope creation.");

        using var scope = scopeProvider.CreateScope();
        var stateRules = scope.GetServices<IStateRule>();

        logger ??= scope.GetService<ILogger>();

        var failures = new List<StateRuleFailure>();

        var task = Task.Run(async () =>
        {
            var cancellationToken = CancellationToken.None;
            foreach (var rule in stateRules)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var ruleName = rule.GetType().FullName ?? rule.GetType().Name;

                try
                {
                    if (await rule.ValidateAsync(cancellationToken).ConfigureAwait(false))
                    {
                        logger?.LogDebug($"State rule {ruleName} is valid.", ruleName);
                        continue;
                    }

                    logger?.LogWarn($"State rule {ruleName} is invalid, attempting to fix.",
                        ruleName);
                    await rule.TryFixAsync(cancellationToken).ConfigureAwait(false);

                    if (!await rule.ValidateAsync(cancellationToken).ConfigureAwait(false))
                    {
                        throw new InvalidOperationException(
                            $"State rule {ruleName} completed TryFixAsync without throwing, " +
                            "but is still invalid after re-validation.");
                    }

                    logger?.LogInfo($"State rule {ruleName} was successfully fixed.", ruleName);
                }
                catch (Exception ex)
                {
                    logger?.LogError($"State rule {ruleName} failed on ", ruleName, ex);
                    failures.Add(new StateRuleFailure(ruleName, ex));
                }
            }
        });

        task.Wait();

        return failures.Count > 0
            ? throw new StateRulesValidationException(failures)
            : scopeProvider;
    }
}