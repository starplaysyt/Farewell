using Farewell.Abstractions.General;

namespace Farewell.Abstractions.Validation;

public sealed class ValidationStatus(ResultCode code, string propertyName)
{
    public static readonly ValidationStatus Ok = new(ResultCode.Ok, string.Empty);

    public ResultCode Code { get; } = code;
    public string PropertyName { get; } = propertyName;
    public bool IsSuccess => Code == ResultCode.Ok;

    public static ValidationStatus Error(ResultCode code, string propertyName)
        => new(code, propertyName);
}