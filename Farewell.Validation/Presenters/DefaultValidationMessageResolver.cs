using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Presenters;

public sealed class DefaultValidationMessageResolver : IValidationMessageResolver
{
    private static readonly Dictionary<uint, string> _defaults = new()
    {
        [100] = "'{0}' must not be empty.",
        [101] = "'{0}' is required.",
        [200] = "'{0}' is too short.",
        [201] = "'{0}' is too long.",
        [202] = "'{0}' has invalid format.",
        [203] = "'{0}' is not a valid email.",
        [204] = "'{0}' is not a valid phone number.",
        [300] = "'{0}' is out of range.",
        [301] = "'{0}' must be positive.",
        [302] = "'{0}' must be negative.",
        [400] = "'{0}' must be unique.",
        [401] = "'{0}' was not found.",
        [500] = "'{0}' references an invalid entity.",
        [600] = "'{0}' conflicts with existing data.",
        [601] = "'{0}' is forbidden.",
        [602] = "'{0}' has expired."
    };

    public string Resolve(ValidationCode code, string propertyName)
        => _defaults.TryGetValue(code.Value, out var template)
            ? string.Format(template, propertyName)
            : $"'{propertyName}' failed validation (code {code.Value}).";
}