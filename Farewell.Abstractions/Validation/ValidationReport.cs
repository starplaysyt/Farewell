using Farewell.Abstractions.General;

namespace Farewell.Abstractions.Validation;

public sealed class ValidationReport(ValidationStatus[]? errors)
{
    public static readonly ValidationReport Ok = new(null);

    public ResultCode Status { get; } = errors?.Length switch
    {
        null or 0 => ResultCode.Ok,
        1 => errors[0].Code,
        _ => ResultCode.ValidationMultipleErrors
    };

    public IReadOnlyList<ValidationStatus>? Errors { get; } = errors;
    public bool IsSuccess => Status == ResultCode.Ok;
}