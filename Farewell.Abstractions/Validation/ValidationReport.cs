namespace Farewell.Abstractions.Validation;

public sealed class ValidationReport(ValidationStatus[]? errors)
{
    public static readonly ValidationReport Ok = new(null);

    public ValidationCode Status { get; } = errors?.Length switch
    {
        null => ValidationCode.Ok,
        0 => ValidationCode.Ok,
        1 => errors[0].Code,
        _ => ValidationCode.MultipleErrors
    };

    public IReadOnlyList<ValidationStatus>? Errors { get; } = errors;
    public bool IsSuccess => Status == ValidationCode.Ok;
}