using Farewell.Abstractions.Validation;

namespace Farewell.Validation;

public sealed class ValidationPresenter(IValidationMessageResolver resolver) : IValidationPresenter
{
    public string Format(ValidationStatus status)
    {
        if (status.IsSuccess) return string.Empty;
        return resolver.Resolve(status.Code, status.PropertyName);
    }

    public IReadOnlyList<string> Format(ValidationReport report)
    {
        if (report.IsSuccess || report.Errors is null)
            return [];

        var messages = new string[report.Errors.Count];
        for (var i = 0; i < report.Errors.Count; i++)
            messages[i] = Format(report.Errors[i]);

        return messages;
    }
}