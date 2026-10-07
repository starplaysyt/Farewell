using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Collectors;

public sealed class BreakOnFirstCollector : IValidationCollector
{
    private ValidationStatus? _first;

    public bool Collect(ValidationStatus status)
    {
        if (status.IsSuccess) return true;
        
        _first = status;
        return false;
    }

    public ValidationReport ToReport()
        => _first is not null
            ? new ValidationReport([_first])
            : ValidationReport.Ok;
}