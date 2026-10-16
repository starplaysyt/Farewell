using Farewell.Abstractions.Validation;

namespace Farewell.Validation.Resolvers;

public class DefaultValidationMessageResolver : IValidationMessageResolver
{
    public string Resolve(ValidationCode code, string propertyName)
        => code.Value switch
        {
            100 => $"Field \"{propertyName}\" should not be empty.",
            101 => $"Field \"{propertyName}\" is required.",
            200 => $"Field \"{propertyName}\" is too short.",
            201 => $"Field \"{propertyName}\" is too long.",
            202 => $"Field \"{propertyName}\" has an invalid format.",
            203 => $"Field \"{propertyName}\" is not a correct email.",
            204 => $"Field \"{propertyName}\" is not a correct phone number.",
            300 => $"Field value \"{propertyName}\" is out of bounds.",
            400 => $"Field value \"{propertyName}\" is already exists.",
            401 => $"Object \"{propertyName}\" not found.",
            500 => $"Incorrect reference in \"{propertyName}\"",
            600 => $"Conflict in field \"{propertyName}\"",
            601 => $"Operation is forbidden \"{propertyName}\"",
            602 => $"Field \"{propertyName}\" is expired.",
            _   => $"Validation exception in field \"{propertyName}\" (code {code.Value})."
        };
}