namespace Farewell.Abstractions.Validation;

public interface IValidationConstraint<in TValue>
{
    ValidationCode Code { get; }
    ValueTask<bool> CheckAsync(TValue value, IServiceProvider sp, CancellationToken ct);
}