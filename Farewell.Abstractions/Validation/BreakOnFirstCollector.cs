namespace Farewell.Abstractions.Validation;

public sealed class BreakOnFirstCollector : IValidationCollector
{
    private ValidationStatus? _error;

    public bool Collect(ValidationStatus status)
    {
        if (!status.IsSuccess)
        {
            _error = status;
            return false; // stop immediately
        }
        return true;
    }

    public ValidationReport ToReport()
        => _error is null
            ? ValidationReport.Ok
            : new ValidationReport([_error]);
}