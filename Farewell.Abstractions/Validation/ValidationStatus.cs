namespace Farewell.Abstractions.Validation;

public sealed class ValidationStatus(ValidationCode code, string propertyName)
{
    public static readonly ValidationStatus Ok = new(ValidationCode.Ok, string.Empty);

    public ValidationCode Code { get; } = code;
    public string PropertyName { get; } = propertyName;
    public bool IsSuccess => Code == ValidationCode.Ok;

    public static ValidationStatus Error(ValidationCode code, string propertyName) =>
        new(code, propertyName);

    public override string ToString() => $"{PropertyName} : {Code}";
}