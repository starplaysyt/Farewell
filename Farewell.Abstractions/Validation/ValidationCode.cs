namespace Farewell.Abstractions.Validation;

/// <summary>
/// Code of validation result. Codes from 0-999 are reserved by the library, 1000+ are custom.
/// </summary>
/// <param name="Value"></param>
public record struct ValidationCode(uint Value) : IEquatable<ValidationCode>
{
    public uint Value { get; } = Value;

    public static readonly ValidationCode Ok = new(0);
    public static readonly ValidationCode MultipleErrors = new(1);

    public static readonly ValidationCode NotEmpty = new(100);
    public static readonly ValidationCode Required = new(101);

    public static readonly ValidationCode MinLength = new(200);
    public static readonly ValidationCode MaxLength = new(201);
    public static readonly ValidationCode InvalidFormat = new(202);
    public static readonly ValidationCode InvalidEmail = new(203);
    public static readonly ValidationCode InvalidPhone = new(204);

    public static readonly ValidationCode OutOfRange = new(300);
    public static readonly ValidationCode MustBePositive = new(301);
    public static readonly ValidationCode MustBeNegative = new(302);

    public static readonly ValidationCode NotUnique = new(400);
    public static readonly ValidationCode NotFound = new(401);

    public static readonly ValidationCode InvalidReference = new(500);

    public static readonly ValidationCode Conflict = new(600);
    public static readonly ValidationCode Forbidden = new(601);
    public static readonly ValidationCode Expired = new(602);
    public static implicit operator uint(ValidationCode code) => code.Value;
    public static explicit operator ValidationCode(uint value) => new(value);
}