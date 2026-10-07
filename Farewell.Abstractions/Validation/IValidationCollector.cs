namespace Farewell.Abstractions.Validation;

public interface IValidationCollector
{
    /// <summary>
    /// Processes the result of one rule.
    /// </summary>
    /// <param name="status"></param>
    /// <returns>true to continue, false to stop.</returns>
    bool Collect(ValidationStatus status);

    /// <summary>
    /// Returns report based on executed collections.
    /// </summary>
    /// <returns>ValidationReport of all passed rules.</returns>
    ValidationReport ToReport();
}