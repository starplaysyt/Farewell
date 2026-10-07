using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Collectors;

public sealed class CollectAllCollector : IValidationCollector
{
    private List<ValidationStatus>? _errors;

    public bool Collect(ValidationStatus status)
    {
        if (status.IsSuccess) return true;
        
        _errors ??= [];
        _errors.Add(status);
        return true;
    }

    public ValidationReport ToReport()
        => _errors is null
            ? ValidationReport.Ok
            : new ValidationReport([.. _errors]);
}