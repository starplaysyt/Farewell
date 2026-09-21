namespace Farewell.Abstractions.Validation;

public enum ValidationCode : uint
{
    Ok = 0,
    MultipleErrors = 1,
    NotEmpty = 2,
    MinLength = 3,
    MaxLength = 4,
    OutOfRange = 5,
    InvalidFormat = 6
}