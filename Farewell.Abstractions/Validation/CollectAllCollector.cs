namespace Farewell.Abstractions.Validation;

public sealed class CollectAllCollector : IValidationCollector
{
    private readonly List<ValidationStatus> _errors = [];

    public bool Collect(ValidationStatus status)
    {
        if (!status.IsSuccess)
            _errors.Add(status);
        return true; // always continue
    }

    public ValidationReport ToReport()
        => _errors.Count == 0
            ? ValidationReport.Ok
            : new ValidationReport([.. _errors]);
}