namespace Farewell.Abstractions.StateRules;

public interface IStateRule
{
    Task<bool> ValidateAsync(CancellationToken cancellationToken = default);
    Task TryFixAsync(CancellationToken cancellationToken = default);
}