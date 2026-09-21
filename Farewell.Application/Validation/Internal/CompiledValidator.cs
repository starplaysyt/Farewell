using Farewell.Abstractions.Validation;

namespace Farewell.Application.Validation.Internal;

internal sealed class CompiledValidator<T> : IValidator<T>
{
    private readonly Func<T, ValidationReport> _validateAll;
    private readonly Func<T, ValidationStatus> _validateBreak;

    internal CompiledValidator(
        Func<T, ValidationReport> validateAll,
        Func<T, ValidationStatus> validateBreak)
    {
        _validateAll = validateAll;
        _validateBreak = validateBreak;
    }

    public ValidationReport ValidateAll(T instance) => _validateAll(instance);
    public ValidationStatus ValidateBreak(T instance) => _validateBreak(instance);
}